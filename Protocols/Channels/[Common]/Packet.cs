namespace Protocols.Channels
{
    using System.Collections.Generic;

    public abstract class Packet : IPacket
    {
        #region Properties

        public IDictionary<string, string> Unspecific { get; }

        public IDictionary<int, string> UnspecificRows { get; }

        public bool UseCompact { get; set; }

        #endregion Properties

        #region Constructors

        public Packet(bool compact = false)
        {
            UnspecificRows = new Dictionary<int, string>();
            Unspecific = new Dictionary<string, string>();
            UseCompact = compact;
        }

        #endregion Constructors

        #region Methods

        public abstract void Pack(ref byte[] buffer, ref int offset);

        public abstract void Unpack(byte[] buffer, ref int offset, int count);

        #endregion Methods
    }
}