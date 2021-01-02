namespace Protocols.Tests
{
    using System;
    using System.Text;
    using NUnit.Framework;

    using Protocols.Channels.Sdp;

    [TestFixture]
    public class SdpMessageTests
    {
        #region Fields

        private const string AudioMessage =
            "v=0 \r\n" +
            "o=root 1350070557 1350070557 IN IP4 192.168.56.105 \r\n" +
            "s=Asterisk PBX 11.7.0~dfsg-1ubuntu1 \r\n" +
            "c=IN IP4 192.168.56.105 \r\n" +
            "t=0 0 \r\n" +
            "m=audio 19516 RTP/AVP 0 8 101 \r\n" +
            "a=rtpmap:0 PCMU/8000 \r\n" +
            "a=rtpmap:8 PCMA/8000 \r\n" +
            "a=rtpmap:101 telephone-event/8000 \r\n" +
            "a=fmtp:101 0-16 \r\n" +
            "a=ptime:20 \r\n" +
            "a=sendrecv \r\n";

        private SdpMessage _packet;

        #endregion Fields

        [SetUp]
        public void SetUp() 
        {
            _packet = new SdpMessage();
        }

        #region Methods

        [Test]
        public void Pack() 
        {
            // Arrage
            // Act
            // Assert
            Assert.IsTrue(false);
        }

        [TestCase(AudioMessage)]
        public void Unpack(string message)
        {
            // Arrage
            var offset = 0;
            byte[] bytes = Encoding.ASCII.GetBytes(message);

            // Act
            bool result = _packet.TryUnpack(bytes, ref offset);

            // Assert
            Assert.AreEqual(true, result);
            Assert.AreEqual(289, offset);
            Assert.AreEqual("0", _packet.Version);
            Assert.AreEqual("root", _packet.Origin.User);
            Assert.AreEqual(1350070557, _packet.Origin.Id);
            Assert.AreEqual(1350070557, _packet.Origin.Version);
            Assert.AreEqual("IN", _packet.Origin.NetType);
            Assert.AreEqual("IP4", _packet.Origin.AddrType);
            Assert.AreEqual("192.168.56.105", _packet.Origin.Address.ToString());
            Assert.AreEqual("Asterisk PBX 11.7.0~dfsg-1ubuntu1", _packet.SessionName);
            Assert.AreEqual("IN", _packet.ConnectionData.NetType);
            Assert.AreEqual("IP4", _packet.ConnectionData.AddrType);
            Assert.AreEqual("192.168.56.105", _packet.ConnectionData.Address.ToString());
            Assert.AreEqual(0, _packet.ConnectionData.TimeToLive);
            Assert.AreEqual(0, _packet.Timing.Start);
            Assert.AreEqual(0, _packet.Timing.Stop);
        }

        #endregion Methods
    }
}
