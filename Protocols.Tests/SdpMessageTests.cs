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
            //"m=audio 19516/2 RTP/AVP 0 8 101 \r\n" +
            "a=rtpmap:0 PCMU/8000 \r\n" +
            "a=rtpmap:8 PCMA/8000 \r\n" +
            "a=rtpmap:101 telephone-event/8000 \r\n" +
            "a=fmtp:101 0-16 \r\n" +
            "a=ptime:20 \r\n" +
            "a=sendrecv \r\n" +
            " \r\n";

        private SdpMessage _packet;

        #endregion Fields

        [SetUp]
        public void SetUp() 
        {
            _packet = new SdpMessage();
        }

        #region Methods

        [TestCase(AudioMessage)]
        public void PackTest(string message) 
        {
            // Arrange
            UnpackTest(message);

            var lines = message.Split("\r\n");
            var bytes = new byte[0];
            var offset = 0;

            // Act
            _packet.Pack(ref bytes, ref offset);

            // Assert
            var newLine = Encoding.ASCII.GetString(bytes).Split("\r\n");

            Assert.AreNotEqual(offset, 0);
            Assert.AreEqual(offset, bytes.Length);
            CollectionAssert.AreEquivalent(lines, newLine);
        }

        [TestCase(AudioMessage)]
        public void UnpackTest(string message)
        {
            // Arrange
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            var offset = 0;

            // Act
            _packet.Unpack(bytes, ref offset, bytes.Length);

            // Assert
            Assert.AreEqual(292, offset);
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
            // Unpack media.
        }

        #endregion Methods
    }
}
