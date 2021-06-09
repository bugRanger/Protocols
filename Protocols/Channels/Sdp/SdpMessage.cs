﻿namespace Protocols.Channels.Sdp
{
    using System;
    using System.Text;

    using SdpTag = PacketItem<SdpMessage>;

    //v=0
    //o=- 1815849 0 IN IP4 194.167.15.181
    //s=Cisco SDP 0
    //c=IN IP4 194.167.15.181
    //t=0 0
    //m=audio 20062 RTP/AVP 99 18 101 100
    //a=rtpmap:99 G.729b/8000
    //a=rtpmap:101 telephone-event/8000
    //a=fmtp:101 0-15
    //a=rtpmap:100 X-NSE/8000
    //a=fmtp:100 200-202

    public class SdpMessage : Packet
    {
        #region Constants

        private const string SPACE = " ";
        private const string CRLF = "\r\n";
        private const string EQUAL = "=";
        private const string VERSION = "0";

        #endregion Constants

        #region Fields

        private static readonly PacketBuilder<SdpMessage> _builder;

        #endregion Fields

        #region Properties

        public string Version { get; private set; }

        public SdpOrigin Origin { get; set; }

        public string SessionName { get; set; }

        public SdpConnectionData ConnectionData { get; set; }

        public SdpTiming Timing { get; set; }

        #endregion Properties

        #region Constructors

        static SdpMessage()
        {
            _builder = new PacketBuilder<SdpMessage>(6)
            {
                new SdpTag("v", (packet) => packet.Version, (packet, value) => packet.Version = value),
                new SdpTag("o", (packet) => packet.Origin.Pack(), (packet, value) => packet.Origin = SdpOrigin.Parse(value)),
                new SdpTag("s", (packet) => packet.SessionName, (packet, value) => packet.SessionName = value),
                new SdpTag("c", (packet) => packet.ConnectionData.Pack(), (packet, value) => packet.ConnectionData = SdpConnectionData.Parse(value)),
                new SdpTag("t", (packet) => packet.Timing.Pack(), (packet, value) => packet.Timing = SdpTiming.Parse(value)),
                //new SdpProperty("m", (packet) => packet.Version, (packet, value) => packet.Version = value),
            };
            _builder.Encoding = Encoding.ASCII;
            _builder.Equal = EQUAL;
            _builder.Separator = SPACE + CRLF;
            _builder.TrailingSeparator = true;
        }

        #endregion Constructors

        #region Methods

        public override void Pack(ref byte[] buffer, ref int offset)
        {
            _builder.Pack(this, ref buffer, ref offset);
        }

        public override void Unpack(byte[] buffer, ref int offset, int count)
        {
            var tmpOffset = offset;

            _builder.Unpack(this, buffer, ref tmpOffset, count);

            if (Version != VERSION)
            {
                throw new ArgumentException(nameof(Version));
            }

            offset = tmpOffset;
        }

        public ArraySegment<byte> Pack()
        {
            var buffer = new byte[0];
            var offset = 0;

            Pack(ref buffer, ref offset);

            return new ArraySegment<byte>(buffer, 0, offset);
        }

        #endregion Methods
    }
}
