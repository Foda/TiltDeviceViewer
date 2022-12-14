namespace TiltViewer.Beacon
{
    internal interface IBeaconService
    {
        bool IsScanning { get; }
        event EventHandler<BeaconData> OnRecievedBeaconData;
        void Start();
        void Stop();
    }
}
