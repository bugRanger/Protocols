namespace Protocols.Channels.Sdp
{
    using System.ComponentModel;

    public enum ProtocolType
    {
        // denotes an unspecified protocol running over UDP.
        [Description("udp")]
        Udp,

        // denotes RTP [19] used under the RTP Profile for Audio
        // and Video Conferences with Minimal Control [20] running over
        // UDP.
        [Description("RTP/AVP")]
        RtpAvp,

        // denotes the Secure Real-time Transport Protocol [23]
        // running over UDP.
        [Description("RTP/SAVP")]
        RtpSavp,
    }
}
