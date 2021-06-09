namespace Protocols.Channels.Sip
{
    using System;
    using System.Linq;

    public class SipVias : PacketItem<SipPacket>
    {
        #region Constructors

        public SipVias() : base(Getter, Setter)
        {
            Name = "Via";
            CompactName = "v";
        }

        #endregion Constructors

        #region Methods

        private static string Getter(SipPacket packet)
        {
            return string.Join(SipPacket.CRLF, packet.Via.Where(w => !w.IsEmpty()).Select(s => s.Pack()));
        }

        private static void Setter(SipPacket packet, string value)
        {
            packet.Via.Add(SipVia.Parse(value));
        }

        #endregion Methods
    }
}
