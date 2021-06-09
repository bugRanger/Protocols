namespace Protocols.Channels.Sip
{
    using System;

    public class SipCSeq : PacketItem<SipPacket>
    {
        #region Constants

        internal const string CRLF = "\r\n";
        internal const string SPACE = " ";

        #endregion Constants

        #region Constructors

        public SipCSeq() : base(Getter, Setter)
        {
            Name = "CSeq";
        }

        #endregion Constructors

        #region Methods

        private static string Getter(SipPacket packet)
        {
            return packet.CSeq + SPACE + packet.Method;
        }

        private static void Setter(SipPacket packet, string value)
        {
            string[] items = value.Split(SPACE, 2, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length != 2)
                throw new ArgumentException();

            if (!int.TryParse(items[0], out var cseq))
                throw new ArgumentException();

            Enum.TryParse<SipMethod>(items[1], out var method);

            packet.Method = method;
            packet.CSeq = cseq;
        }

        #endregion Methods
    }
}
