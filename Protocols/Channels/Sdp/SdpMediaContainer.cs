namespace Protocols.Channels.Sdp
{
    using System;
    using System.Collections.Generic;

    //m=audio 19516 RTP/AVP 0 8 101 \r\n
    //a=rtpmap:<payload type> <encoding name>/<clock rate> [/<encoding parameters>]
    //a=fmtp:<format> <format specific parameters>
    //a=ptime:<packet time>
    //a=recvonly/sendrecv/sendonly/inactive

    public class SdpMediaContainer
    {
        #region Properties

        public MediaKind Kind { get; set; }

        public int Port { get; set; }

        public int PortCount { get; set; }

        public ProtocolType Protocol { get; set; }

        public int? PacketTime { get; set; }

        public MediaMode Mode { get; set; }

        public IDictionary<int, SdpMediaFormat> Formats { get; }

        #endregion Properties

        #region Constructors

        public SdpMediaContainer()
        {
            Formats = new Dictionary<int, SdpMediaFormat>();
        }

        #endregion Constructors
    }
}
