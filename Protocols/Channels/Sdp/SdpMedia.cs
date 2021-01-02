namespace Protocols.Channels.Sdp
{
    using System;


    //a=rtpmap:<payload type> <encoding name>/<clock rate> [/<encoding parameters>]
    //a=fmtp:<format> <format specific parameters>
    //a=ptime:<packet time>
    //a=recvonly/sendrecv/sendonly/inactive

    public enum MediaMode 
    {
        RecvOnly,
        SendOnly,
        SendRecv,
        InActive,
    }

    public class SdpMedia
    {
        #region Properties

        public byte PayloadType { get; set; }

        public string EncodingName { get; set; }

        public string ClockRate { get; set; }

        public string[] EncodingParams { get; set; }

        public MediaMode Mode { get; set; }

        #endregion Properties

        #region Methods



        #endregion Methods
    }
}
