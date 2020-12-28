namespace Protocols.Channels.Sip
{
    using System;

    //Example:          SIP/2.0/UDP 192.168.56.1:5061;branch=z9hG4bKPj3e715762683b4c95b5609f613dcee8bf;received=192.168.56.1
    //via-params        =  via-ttl / via-maddr / via-received / via-branch / response-port / via-extension

    public class SipVia : IBuildPacket, IEquatable<SipVia>
    {
        #region Constants

        private const string EQUAL = "=";
        private const string SEPARATOR = ";";

        #endregion Constants

        #region Fields

        private static readonly PacketBuilder<SipVia> _properties;

        #endregion Fields

        #region Properties

        public string Protocol { get; set; }

        public string Aliase { get; set; }

        public string Branch { get; set; }

        public string Received { get; set; }

        public int? ResponsePort { get; set; }

        public bool UseCompact => false;

        #endregion Properties

        #region Constructors

        static SipVia() 
        {
            _properties = new PacketBuilder<SipVia>()
            {
                new PacketProperty<SipVia>("rport", (p) => p.ResponsePort.ToString(), (p, value) => p.ResponsePort = int.Parse(value), (p) => p.ResponsePort.HasValue),
                new PacketProperty<SipVia>("branch", (p) => p.Branch, (p, value) => p.Branch = value, (p) => !string.IsNullOrWhiteSpace(p.Branch)),
                new PacketProperty<SipVia>("received", (p) => p.Received, (p, value) => p.Received = value, (p) => !string.IsNullOrWhiteSpace(p.Received)),
            };
        }

        #endregion Constructors

        #region Methods

        public string Pack() 
        {
            var result = $"{SipPacket.VERSION}/{Protocol} {Aliase}";

            _properties.Build(this, (property, value) => result += $"{SEPARATOR}{property}{EQUAL}{value}");

            return result;
        }

        public static SipVia Parse(string via)
        {
            var result = new SipVia();

            if (!via.StartsWith(SipPacket.VERSION))
            {
                return null;
            }

            var token = via.Substring(SipPacket.VERSION.Length + 1)
                .Split(SipPacket.SPACE, StringSplitOptions.RemoveEmptyEntries);

            if (token.Length < 1)
            {
                return null;
            }

            result.Protocol = token[0];

            token = token[1].Split(SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
            if (token.Length == 0)
            {
                return null;
            }

            result.Aliase = token[0];

            if (token.Length != 1)
            {
                for (int i = 1; i < token.Length; i++)
                {
                    var subToken = token[i].Split(EQUAL);
                    if (subToken.Length != 2)
                    {
                        continue;
                    }

                    if(!_properties.TryGetValue(subToken[0], out var property))
                    {
                        continue;
                    }

                    property.Set(result, subToken[1]);
                }
            }


            return result;
        }

        internal bool IsEmpty()
        {
            return
                string.IsNullOrEmpty(Protocol) ||
                string.IsNullOrEmpty(Aliase);
        }

        public bool Equals(SipVia other)
        {
            return
                other != null &&
                Protocol == other.Protocol &&
                Aliase == other.Aliase &&
                Branch == other.Branch;
        }

        #endregion Methods
    }
}
