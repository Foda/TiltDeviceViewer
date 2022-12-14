using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using TiltViewer.Beacon;

namespace TiltViewer.ViewModels
{
    public class TiltHydrometerViewModel : ReactiveObject
    {
        private BeaconData _beaconData;
        public BeaconData BeaconData
        {
            get => _beaconData;
            private set => this.RaiseAndSetIfChanged(ref _beaconData, value);
        }

        private string _colorName = "";
        public string ColorName
        {
            get => _colorName;
            private set => this.RaiseAndSetIfChanged(ref _colorName, value);
        }

        private Color _color;
        public Color Color
        {
            get => _color;
            private set => this.RaiseAndSetIfChanged(ref _color, value);
        }

        private string _batchName = "Untitled";
        public string BatchName
        {
            get => _batchName;
            private set => this.RaiseAndSetIfChanged(ref _batchName, value);
        }

        private readonly ObservableAsPropertyHelper<float> _specificGravity;
        public float SpecificGravity => _specificGravity.Value;

        private readonly ObservableAsPropertyHelper<int> _temperature;
        /// <summary>
        /// Temperature in degrees F
        /// </summary>
        public int Temperature => _temperature.Value;

        private readonly ObservableAsPropertyHelper<DateTime> _lastUpdate;
        public DateTime LastUpdate => _lastUpdate.Value;

        public ReactiveCommand<Unit, Unit> LogDataCommand { get; }

        public string DataPath =>
            Path.Combine(FileSystem.AppDataDirectory, $"{BatchName}_Tilt{ColorName}.csv");

        public TiltHydrometerViewModel(BeaconData beacon)
        {
            if (Utils.UUIDToColorType.TryGetValue(beacon.FormattedUuid, out Tuple<string, Color> colorLookup))
            {
                this.ColorName = colorLookup.Item1;
                this.Color = colorLookup.Item2;
            }
            else
            {
                throw new ArgumentException("Unable to find Tilt color UUID");
            }
            this.BeaconData = beacon;

            this.LogDataCommand = ReactiveCommand.CreateFromTask(LogLatestData);

            this.WhenAnyValue(x => x.BeaconData, (val) => val.Minor / 1000.0f)
                .ToProperty(this, vm => vm.SpecificGravity, out _specificGravity);

            this.WhenAnyValue(x => x.BeaconData, (val) => (int)val.Major)
                .ToProperty(this, vm => vm.Temperature, out _temperature);

            this.WhenAnyValue(x => x.BeaconData)
                .Select(x => DateTime.Now)
                .ToProperty(this, vm => vm.LastUpdate, out _lastUpdate);

            // Log data to file every 30 seconds on a beacon update, but only if we have a valid name
            this.WhenAnyValue(x => x.BeaconData, x => x.BatchName)
                .Where((x) => !string.IsNullOrEmpty(x.Item2) && x.Item2 != "Untitled")
                .Throttle(TimeSpan.FromSeconds(30))
                .InvokeCommand(LogDataCommand);

            this.RaisePropertyChanged(nameof(BeaconData));
        }

        public void LogData(BeaconData data)
        {
            this.BeaconData = data;
        }

        public void SetBatchName(string batchName)
        {
            BatchName = batchName;
        }

        private async Task LogLatestData()
        {
            StringBuilder sb = new();
            sb.Append(LastUpdate.ToString());
            sb.Append(',');
            sb.Append(SpecificGravity);
            sb.Append(',');
            sb.Append(Temperature);
            sb.Append(',');
            sb.Append(ColorName);
            sb.Append(',');
            sb.Append(BatchName);

            using FileStream outputStream = File.OpenWrite(DataPath);
            using StreamWriter streamWriter = new(outputStream);
            await streamWriter.WriteLineAsync(sb.ToString());
        }
    }
}
