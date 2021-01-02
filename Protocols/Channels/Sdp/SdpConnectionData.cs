namespace Protocols.Channels.Sdp
{
    using System;
    using System.Net;

    // https://tools.ietf.org/html/rfc4566#section-5.7
    //c=<nettype> <addrtype> <connection-address>
    //c=IN IP4 194.167.15.181
    public class SdpConnectionData
    {
        #region Constants

        public const string SEPARATOR = " ";
        public const string DELIMITER = "/";

        #endregion Constants

        #region Properties

        public string NetType { get; set; }

        public string AddrType { get; set; }

        public IPAddress Address { get; set; }

        public byte TimeToLive { get; set; }

        #endregion Properties

        #region Methods

        public string Pack()
        {
            var result = string.Empty;

            result += 
                NetType + SEPARATOR + 
                AddrType + SEPARATOR + 
                Address;

            if (TimeToLive > 0)
                result += DELIMITER + TimeToLive;

            return result;
        }

        public static SdpConnectionData Parse(string line)
        {
            var tokens = line.Split(SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 3)
            {
                throw new ArgumentException();
            }

            int offset = 0;
            var result = new SdpConnectionData
            {
                NetType = tokens[offset++],
                AddrType = tokens[offset++],
            };

            tokens = tokens[offset++].Split(DELIMITER, StringSplitOptions.RemoveEmptyEntries);
            result.Address = IPAddress.Parse(tokens[0]);
            if (tokens.Length == 2) 
            {
                result.TimeToLive = byte.Parse(tokens[1]);
            }

            return result;
        }

        #endregion Methods
    }
}
