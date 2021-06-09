namespace Protocols.Channels.Sdp
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    //m=audio 19516 RTP/AVP 0 8 101 \r\n
    //a=rtpmap:<payload type> <encoding name>/<clock rate> [/<encoding parameters>]
    //a=fmtp:<format> <format specific parameters>
    //a=ptime:<packet time>
    //a=recvonly/sendrecv/sendonly/inactive

    public class SdpMediaContainer
    {
        #region Constants

        private const string SPACE = " ";
        private const string CRLF = "\r\n";

        #endregion Constants

        #region Properties

        public MediaKind Kind { get; set; }

        public List<int> Ports { get; set; }

        public ProtocolType Protocol { get; set; }

        public int? PacketTime { get; set; }

        public MediaMode Mode { get; set; }

        public IDictionary<int, SdpMediaFormat> Formats { get; }

        #endregion Properties

        #region Constructors

        public SdpMediaContainer()
        {
            Ports = new List<int>();
            Formats = new Dictionary<int, SdpMediaFormat>();
        }

        #endregion Constructors

        #region Methods
        
        public string Pack()
        {
            var result = string.Empty;

            //result += 
            //    Kind.+ CRLF+
            //    $"{Ports.FirstOrDefault()}{(Ports.Count > 1 ? $"/{Ports.Count}" : string.Empty)}" + CRLF +
            //    //Protocol.ToString().ToLower() +

            //    "a=ptime:" + PacketTime.ToString().ToLower() + CRLF +
            //    "a=" + Mode.ToString().ToLower() + CRLF;

            return result;
        }

        public static SdpMediaContainer Parse(string line)
        {
            throw new Exception();
        }

        #endregion Methods
    }
}
