namespace TiltViewer.Beacon
{
    public static class Utils
    {
        public static readonly Dictionary<string, Tuple<string, Color>> UUIDToColorType = new Dictionary<string, Tuple<string, Color>>
        {
            { "A495BB10C5B14B44B5121370F02D74DE", new Tuple<string, Color>("Red", Color.FromRgba(192, 43, 28, 255)) },
            { "A495BB20C5B14B44B5121370F02D74DE", new Tuple<string, Color>("Green", Color.FromRgba(15, 123, 15, 255)) },
            { "A495BB30C5B14B44B5121370F02D74DE", new Tuple<string, Color>("Black", Color.FromRgba(4, 4, 4, 255)) },
            { "A495BB40C5B14B44B5121370F02D74DE", new Tuple<string, Color>("Purple", Color.FromRgba(132, 80, 210, 255)) },
            { "A495BB50C5B14B44B5121370F02D74DE", new Tuple<string, Color>("Orange", Color.FromRgba(249, 126, 60, 255)) },
            { "A495BB60C5B14B44B5121370F02D74DE", new Tuple<string, Color>("Blue", Color.FromRgba(0, 120, 212, 255)) },
            { "A495BB70C5B14B44B5121370F02D74DE", new Tuple<string, Color>("Yellow", Color.FromRgba(250, 217, 70, 255)) },
            { "A495BB80C5B14B44B5121370F02D74DE", new Tuple<string, Color>("Pink", Color.FromRgba(204, 53, 149, 255)) },
        };

        public static bool IsValidTiltDevice(BeaconData beacon)
        {
            return UUIDToColorType.ContainsKey(beacon.FormattedUuid);
        }
    }
}
