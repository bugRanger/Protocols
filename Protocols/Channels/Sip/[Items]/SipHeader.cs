namespace Protocols.Channels.Sip
{
    using System;
    using Protocols.Channels;

    public class SipHeader : PacketItem<SipPacket>
    {
        #region Constants

        internal const string CRLF = "\r\n";
        internal const string SPACE = " ";
        internal const string VERSION = "SIP/2.0";

        #endregion Constants

        #region Constructors

        public SipHeader() : base(Getter, Setter)
        {
        }

        #endregion Constructors

        #region Methods

        private static string Getter(SipPacket packet)
        {
            string result;

            if (packet.Status != null)
            {
                result = $"{VERSION} {(int)packet.Status.Value} {((SipStatus)packet.Status).Pack()}";
            }
            else
            {
                result = $"{packet.Method} {packet.Request.Pack(false)} {VERSION}";
            }

            return result;
        }

        private static void Setter(SipPacket packet, string message)
        {
            string[] items = message.TrimEnd().Split(SPACE, 3, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length != 3)
            {
                return;
            }

            if (items[0] == VERSION && int.TryParse(items[^2], out var status) && Enum.IsDefined(typeof(SipStatus), status))
            {
                packet.Status = (SipStatus)status;
            }
            else if (items[^1] == VERSION && Enum.TryParse(items[0], true, out SipMethod method))
            {
                packet.Request = SipUri.Parse(items[1]);
                packet.Method = method;
            }
        }

        #endregion Methods
    }
}
