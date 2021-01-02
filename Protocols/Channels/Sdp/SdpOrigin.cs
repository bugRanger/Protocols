namespace Protocols.Channels.Sdp
{
    using System;
    using System.Net;

    // https://tools.ietf.org/html/rfc4566#section-5.2
    //o=<username> <sess-id> <sess-version> <nettype> <addrtype> <unicast-address>
    //o=- 1815849 0 IN IP4 194.167.15.181
    public class SdpOrigin
    {
        #region Constants

        public const string SEPARATOR = " ";

        #endregion Constants

        #region Properties

        public string User { get; set; }

        public int Id { get; set; }

        public int Version { get; set; }

        public string NetType { get; set; }

        public string AddrType { get; set; }

        public IPAddress Address { get; set; }

        #endregion Properties

        #region Constructors

        #endregion Constructors

        #region Methods

        public string Pack() 
        {
            var result = string.Empty;

            result += string.IsNullOrWhiteSpace(User) ? "-" : User;

            result += 
                SEPARATOR + Id + 
                SEPARATOR + Version + 
                SEPARATOR + NetType + 
                SEPARATOR + AddrType + 
                SEPARATOR + Address;

            return result;
        }

        public static SdpOrigin Parse(string line) 
        {
            var tokens = line.Split(SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 6)
            {
                throw new ArgumentException();
            }

            int offset = 0;
            var result = new SdpOrigin
            {
                User = tokens[offset++],
                Id = int.Parse(tokens[offset++]),
                Version = int.Parse(tokens[offset++]),
                NetType = tokens[offset++],
                AddrType = tokens[offset++],
                Address = IPAddress.Parse(tokens[offset++]),
            };

            return result;
        }

        #endregion Methods
    }
}
