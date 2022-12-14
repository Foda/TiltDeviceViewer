namespace TiltViewer.Beacon
{
    public class BeaconData
    {
        public ulong DeviceAddress { get; private set; }
        public Guid Uuid { get; private set; }
        public string FormattedUuid
        {
            get { return Uuid.ToString().Replace("-", string.Empty).ToUpper(); }
        }
        public ushort Major { get; private set; }
        public ushort Minor { get; private set; }
        public sbyte TxPower { get; private set; }

        public static BeaconData FromBytes(byte[] bytes, ulong deviceAddress)
        {
            if (bytes[0] != 0x02) { throw new ArgumentException("First byte in array was exptected to be 0x02", "bytes"); }
            if (bytes[1] != 0x15) { throw new ArgumentException("Second byte in array was expected to be 0x15", "bytes"); }
            if (bytes.Length != 23) { throw new ArgumentException("Byte array length was expected to be 23", "bytes"); }

            return new BeaconData
            {
                DeviceAddress = deviceAddress,
                Uuid = new Guid(
                        BitConverter.ToInt32(bytes.Skip(2).Take(4).Reverse().ToArray(), 0),
                        BitConverter.ToInt16(bytes.Skip(6).Take(2).Reverse().ToArray(), 0),
                        BitConverter.ToInt16(bytes.Skip(8).Take(2).Reverse().ToArray(), 0),
                        bytes.Skip(10).Take(8).ToArray()),

                Major = BitConverter.ToUInt16(bytes.Skip(18).Take(2).Reverse().ToArray(), 0),
                Minor = BitConverter.ToUInt16(bytes.Skip(20).Take(2).Reverse().ToArray(), 0),

                TxPower = (sbyte)bytes[22]
            };
        }
    }
}
