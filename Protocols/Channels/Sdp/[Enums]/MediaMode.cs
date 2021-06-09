namespace Protocols.Channels.Sdp
{
    using System.ComponentModel;

    public enum MediaMode
    {
        [Description("recvonly")]
        RecvOnly,

        [Description("sendonly")]
        SendOnly,

        [Description("sendrecv")]
        SendRecv,

        [Description("inactive")]
        InActive,
    }
}
