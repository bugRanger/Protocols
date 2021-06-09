namespace Protocols.Channels.Sdp
{
    using System.ComponentModel;

    public enum MediaKind
    {
        [Description("audio")]
        Audio,

        [Description("video")]
        Video,

        [Description("text")]
        Text,

        [Description("application")]
        Application,

        [Description("message")]
        Message,
    }
}
