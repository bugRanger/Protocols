namespace Protocols.Channels.Sip
{
    using System;
    using System.Text;

    //Example:          SIP/2.0/UDP 192.168.56.1:5061;branch=z9hG4bKPj3e715762683b4c95b5609f613dcee8bf;received=192.168.56.1
    //via-params        =  via-ttl / via-maddr / via-received / via-branch / response-port / via-extension

    public class SipVia : Packet, IEquatable<SipVia>
    {
        #region Constants

        private const string SEPARATOR = ";";
        private const string EQUAL = "=";

        #endregion Constants

        #region Fields

        private static readonly PacketBuilder<SipVia> _builder;

        #endregion Fields

        #region Properties

        public string Protocol { get; set; }

        public string Aliase { get; set; }

        public string Branch { get; set; }

        public string Received { get; set; }

        public int? ResponsePort { get; set; }

        #endregion Properties

        #region Constructors

        static SipVia()
        {
            _builder = new PacketBuilder<SipVia>(4)
            {
                new PacketGroup<SipVia>(
                    new PacketGroup<SipVia>(
                        new PacketItem<SipVia>((p) => SipPacket.PROT) { HasOrdered = true, HasConstant = true },
                        new PacketItem<SipVia>((p) => SipPacket.VERSION) { HasOrdered = true, HasConstant = true },
                        new PacketItem<SipVia>((p) => p.Protocol, (p, v) => p.Protocol = v) { HasOrdered = true })
                    .SetBuilder(builder =>
                    {
                        builder.Encoding = SipPacket.Encoding;
                        builder.Separator = SipPacket.SLASH;
                        builder.TrailingSeparator = false;
                    }),
                    new PacketItem<SipVia>((p) => p.Aliase, (p, v) => p.Aliase = v) { HasOrdered = true })
                .SetBuilder(builder => 
                {
                    builder.Encoding = SipPacket.Encoding;
                    builder.Separator = SipPacket.SPACE;
                    builder.TrailingSeparator = false;
                }),

                new PacketItem<SipVia>("rport", (p) => p.ResponsePort.ToString(), (p, value) => p.ResponsePort = int.Parse(value), (p) => p.ResponsePort.HasValue),
                new PacketItem<SipVia>("branch", (p) => p.Branch, (p, value) => p.Branch = value, (p) => !string.IsNullOrWhiteSpace(p.Branch)),
                new PacketItem<SipVia>("received", (p) => p.Received, (p, value) => p.Received = value, (p) => !string.IsNullOrWhiteSpace(p.Received)),
            };
            _builder.Encoding = SipPacket.Encoding;
            _builder.Equal = EQUAL;
            _builder.Separator = SEPARATOR;
            _builder.TrailingSeparator = false;
        }

        #endregion Constructors

        #region Methods

        public static SipVia Parse(string message)
        {
            var result = new SipVia();

            var offset = 0;

            result.Unpack(message, ref offset, message.Length);

            return result;
        }

        public override void Pack(ref byte[] buffer, ref int offset)
        {
            _builder.Pack(this, ref buffer, ref offset);
        }

        public string Pack()
        {
            return _builder.Pack(this);
        }

        public override void Unpack(byte[] buffer, ref int offset, int count) 
        {

        }

        public void Unpack(string message, ref int offset, int count)
        {
            _builder.Unpack(this, message, ref offset, count);
        }

        public bool Equals(SipVia other)
        {
            return
                other != null &&
                Protocol == other.Protocol &&
                Aliase == other.Aliase &&
                Branch == other.Branch;
        }

        internal bool IsEmpty()
        {
            return
                string.IsNullOrEmpty(Protocol) ||
                string.IsNullOrEmpty(Aliase);
        }

        #endregion Methods
    }
}
