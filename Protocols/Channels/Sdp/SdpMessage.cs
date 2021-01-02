namespace Protocols.Channels.Sdp
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using Framework.Common;

    using SdpProperty = PacketProperty<SdpMessage>;

    public class SdpMessage : IPacket, IBuildPacket
    {
        //v=0
        //o=- 1815849 0 IN IP4 194.167.15.181
        //s=Cisco SDP 0
        //c=IN IP4 194.167.15.181
        //t=0 0
        //m=audio 20062 RTP/AVP 99 18 101 100
        //a=rtpmap:99 G.729b/8000
        //a=rtpmap:101 telephone-event/8000
        //a=fmtp:101 0-15
        //a=rtpmap:100 X-NSE/8000
        //a=fmtp:100 200-202

        #region Constants

        private const string SPACE = " ";
        private const string CRLF = "\r\n";
        private const string EQUAL = "=";
        private const string VERSION = "0";

        private static readonly Encoding Encoding = Encoding.ASCII;
        private static readonly PacketBuilder<SdpMessage> _properties;

        private readonly Dictionary<int, string> _cached;

        #endregion Constants

        #region Properties

        public string Version { get; private set; }

        public SdpOrigin Origin { get; set; }

        public string SessionName { get; set; }

        public SdpConnectionData ConnectionData { get; set; }

        public SdpTiming Timing { get; set; }

        public bool UseCompact => false;

        #endregion Properties

        #region Constructors

        static SdpMessage()
        {
            _properties = new PacketBuilder<SdpMessage>(CRLF)
            {
                // TODO: Impl others attributes.
                new SdpProperty("v", (packet) => packet.Version, (packet, value) => packet.Version = value),
                new SdpProperty("o", (packet) => packet.Origin.Pack(), (packet, value) => packet.Origin = SdpOrigin.Parse(value)),
                new SdpProperty("s", (packet) => packet.SessionName, (packet, value) => packet.SessionName = value),
                new SdpProperty("c", (packet) => packet.ConnectionData.Pack(), (packet, value) => packet.ConnectionData = SdpConnectionData.Parse(value)),
                new SdpProperty("t", (packet) => packet.Timing.Pack(), (packet, value) => packet.Timing = SdpTiming.Parse(value)),
                //new SdpProperty("m", (packet) => packet.Version, (packet, value) => packet.Version = value),
            };
        }

        public SdpMessage() 
        {
            _cached = new Dictionary<int, string>();
        }

        #endregion Constructors

        #region Methods

        public bool TryUnpack(byte[] buffer, ref int offset)
        {
            var tmpOffset = offset;
            var count = buffer.Length - tmpOffset;
            if (count <= 0)
            {
                return false;
            }

            string[] message = Encoding
                .GetString(buffer, tmpOffset, count)
                .Split(CRLF, StringSplitOptions.RemoveEmptyEntries);

            if (message.Length <= 1)
            {
                return false;
            }

            if (!TryParse(message, ref tmpOffset))
            {
                return false;
            }

            if (Version != VERSION)
            {
                return false;
            }

            offset = tmpOffset;
            return true;
        }

        public void Pack(ref byte[] buffer, ref int offset)
        {
            byte[] message = Encoding.GetBytes(PropertyBuild());

            BufferBits.Prepare(ref buffer, offset, message.Length);
            BufferBits.SetBytes(message, buffer, ref offset);
        }

        public ArraySegment<byte> Pack()
        {
            var buffer = new byte[0];
            var offset = 0;

            Pack(ref buffer, ref offset);

            return new ArraySegment<byte>(buffer, 0, offset);
        }

        private string PropertyBuild()
        {
            var properties = new List<string>();
            _properties.Build(this, (property, value) => properties.Add($"{property}{EQUAL}{value}"));

            if (_cached.Count == 0)
            {
                return string.Join(SPACE + CRLF, properties) + SPACE + CRLF;
            }

            var line = 0;
            var cached = string.Empty;
            var enumeration = properties.GetEnumerator();

            foreach (var item in _cached)
            {
                while (line < item.Key)
                {
                    if (!enumeration.MoveNext())
                        break;

                    cached += enumeration.Current + SPACE + CRLF;
                    line++;
                }

                cached += item.Value + CRLF;
                line++;
            }

            while (enumeration.MoveNext())
            {
                cached += enumeration.Current + SPACE + CRLF;
            }

            return cached + SPACE + CRLF;
        }

        private bool TryParse(string[] message, ref int offset)
        {
            _cached.Clear();

            for (int i = 0; i < message.Length; i++)
            {
                offset += Encoding.GetByteCount(message[i] + CRLF);

                var items = message[i].TrimEnd().Split(EQUAL, 2, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length != 2)
                {
                    return false;
                }

                if (!_properties.TryGetValue(items[0], out var property))
                {
                    _cached.Add(i, message[i]);
                    continue;
                }

                try
                {
                    property.Set(this, items[1].TrimStart());
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(items[0], ex);
                }
            }

            return true;
        }

        #endregion Methods
    }
}
