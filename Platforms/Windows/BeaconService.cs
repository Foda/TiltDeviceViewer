using TiltViewer.Beacon;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;

namespace TiltViewer.Platforms.Windows
{
    public partial class BeaconService : IBeaconService
    {
        private BluetoothLEAdvertisementWatcher _watcher;

        public bool IsScanning
        {
            get
            {
                if (_watcher != null)
                {
                    if (_watcher.Status == BluetoothLEAdvertisementWatcherStatus.Started ||
                        _watcher.Status == BluetoothLEAdvertisementWatcherStatus.Created)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public event EventHandler<BeaconData> OnRecievedBeaconData;

        public void Start()
        {
            _watcher = new BluetoothLEAdvertisementWatcher();
            _watcher.Received += Watcher_Received;
            _watcher.Start();
        }

        private void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            const ushort AppleCompanyId = 0x004C;
            foreach (var adv in args.Advertisement.ManufacturerData.Where(x =>
                x.CompanyId == AppleCompanyId &&
                x.Data.Length >= 23))
            {
                var bytes = new byte[adv.Data.Length];
                using (var reader = DataReader.FromBuffer(adv.Data))
                {
                    reader.ReadBytes(bytes);
                }

                // 2 == iBeacon
                if (bytes[0] != 0x02)
                    continue;

                // 21 bytes for iBeacon
                if (bytes[1] != 0x15)
                    continue;

                OnRecievedBeaconData?.Invoke(this, BeaconData.FromBytes(bytes, args.BluetoothAddress));
            }
        }

        public void Stop()
        {
            _watcher.Stop();
        }
    }
}