using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiltViewer.Beacon;
using TiltViewer.Platforms.Windows;

namespace TiltViewer.ViewModels
{
    public class MainPageViewModel : ReactiveObject
    {
        private IBeaconService _beaconService;

        public ObservableCollection<TiltHydrometerViewModel> TiltViewModels { get; }

        private string _scanText = "Refresh Devices";
        public string ScanText
        {
            get => _scanText;
            private set => this.RaiseAndSetIfChanged(ref _scanText, value);
        }

        private bool _isScanning = false;
        public bool IsScanning
        {
            get => _isScanning;
            private set => this.RaiseAndSetIfChanged(ref _isScanning, value);
        }

        public IReactiveCommand ToggleScan { get; }
        public IReactiveCommand AddMock { get; }

        public MainPageViewModel()
        {
            TiltViewModels = new ObservableCollection<TiltHydrometerViewModel>();

            _beaconService = new BeaconService();
            _beaconService.OnRecievedBeaconData += _beaconService_OnRecievedBeaconData;

            ToggleScan = ReactiveCommand.Create(ToggleBeaconScan);
            AddMock = ReactiveCommand.Create(AddMockData);
        }

        private void ToggleBeaconScan()
        {
            if (!_beaconService.IsScanning)
            {
                _beaconService.Start();
                IsScanning = true;
                ScanText = "Stop";
            }
            else
            {
                _beaconService.Stop();
                IsScanning = false;
                ScanText = "Refresh Devices";
            }
        }

        private void AddMockData()
        {
            BeaconData fakeData = BeaconData.FromBytes(
                new byte[23] { 2, 21, 164, 149, 187, 48, 197, 177, 75, 68, 181, 18, 19, 112, 240, 45, 116, 222, 0, 68, 3, 226, 197 }, 0451);

            TiltViewModels.Add(new TiltHydrometerViewModel(fakeData));
        }

        private void _beaconService_OnRecievedBeaconData(object sender, BeaconData data)
        {
            if (!Utils.IsValidTiltDevice(data))
                return;

            TiltHydrometerViewModel viewModel = TiltViewModels.FirstOrDefault(x => x.BeaconData.DeviceAddress == data.DeviceAddress);
            if (viewModel == null)
            {
                viewModel = new TiltHydrometerViewModel(data);
                TiltViewModels.Add(new TiltHydrometerViewModel(data));
            }
            else
            {
                viewModel.LogData(data);
            }
        }
    }
}
