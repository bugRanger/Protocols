namespace Protocols.Tests
{
    using System;

    using NUnit.Framework;

    using Protocols.Channels.Rtp;

    [TestFixture]
    public class RtpPacketTests
    {
        #region Constants

        private static byte[] _bytes = 
        {
            0x80, 0x00, 0x20, 0x21, 0x12, 0xbe, 0x71, 0x4a,
            0x20, 0x00, 0x00, 0x02, 0xf0, 0x6e, 0x6c, 0xdf,
            0xe2, 0x62, 0x67, 0x6f, 0x78, 0xee, 0x6b, 0xf5,
            0xd5, 0xed, 0x6a, 0xeb, 0xf3, 0x6f, 0x7a, 0x5f,
            0x57, 0x6d, 0x6d, 0x5f, 0xe0, 0xda, 0x77, 0xe6,
            0xda, 0x71, 0x72, 0xe3, 0x71, 0x6f, 0xe7, 0x7c,
            0x6c, 0x74, 0x73, 0xfd, 0xf5, 0x79, 0x61, 0x69,
            0xee, 0x66, 0x64, 0xe4, 0x6c, 0x5c, 0xe6, 0xdf,
            0xeb, 0xec, 0xe2, 0xd8, 0xe8, 0x77, 0xec, 0x6e,
            0x5e, 0x6f, 0x67, 0x58, 0x5f, 0xee, 0x76, 0x5c,
            0xff, 0xdf, 0xea, 0xee, 0xe2, 0xdb, 0xf3, 0x61,
            0xf8, 0xdf, 0x6f, 0x52, 0x5d, 0xdf, 0xdd, 0xe5,
            0xd7, 0xdd, 0x53, 0x48, 0x4e, 0x69, 0xde, 0xdb,
            0xd7, 0xd2, 0xdb, 0xe9, 0xea, 0x74, 0x5a, 0x5b,
            0x5c, 0x5c, 0x67, 0x67, 0x6d, 0xfd, 0x7c, 0xf6,
            0xe1, 0xde, 0xdf, 0xdd, 0xdf, 0xdf, 0xdc, 0xe3,
            0x77, 0x62, 0x5e, 0x58, 0x56, 0x5e, 0x62, 0x5f,
            0x5f, 0x67, 0xf6, 0xe1, 0xd4, 0xcc, 0xd8, 0x67,
            0x60, 0x6e, 0x63, 0x65, 0x7c, 0x78, 0xfc, 0xe3,
            0xd8, 0xd1, 0xd3, 0xd8, 0xea, 0x5b, 0x4e, 0x4b,
            0x4b, 0x4e, 0x52, 0x5d, 0xfc, 0xda, 0xce, 0xce,
            0xd3, 0xe7, 0x7d, 0xec
        };


        #endregion Constants

        #region Fields

        private byte _version;
        private ushort _secno;
        private uint _ssrc;
        private byte _pt;
        private uint _timestamp;
        private ArraySegment<byte> _payload;

        private RtpPacket _packet;

        #endregion Fields

        #region Methods

        [SetUp]
        public void SetUp() 
        {
            _version = 2;
            _secno = 8225;
            _ssrc = 0x20000002;
            _pt = 0;
            _timestamp = 314470730;
            _payload = new ArraySegment<byte>(_bytes, 12, _bytes.Length - 12);

            _packet = new RtpPacket();
        }

        [Test]
        public void UnpackTest()
        {
            // Arrage
            var offset = 0;

            // Act
            bool result = _packet.TryUnpack(_bytes, ref offset);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(offset, _bytes.Length);
            Assert.AreEqual(_packet.Version, _version);
            Assert.AreEqual(_packet.HasPadding, false);
            Assert.AreEqual(_packet.HasExtension, false);
            Assert.AreEqual(_packet.CsrcCount, 0);
            Assert.AreEqual(_packet.Marker, false);
            Assert.AreEqual(_packet.PayloadType, _pt);
            Assert.AreEqual(_packet.SequenceNumber, _secno);
            Assert.AreEqual(_packet.TimeStamp, _timestamp);
            Assert.AreEqual(_packet.Ssrc, _ssrc);
            CollectionAssert.AreEqual(_packet.Payload, _payload.ToArray());
        }

        [Test]
        public void UnpackWithOffsetTest()
        {
            // Arrage
            var offset = 0;

            // Act
            // Assert
        }

        [Test]
        public void PackTest()
        {
            // Arrage
            var bytes = new byte[0];
            var offset = 0;

            // Act
            _packet.Version = _version;
            _packet.HasPadding = false;
            _packet.HasExtension = false;
            _packet.Marker = false;
            _packet.SequenceNumber = _secno;
            _packet.TimeStamp = _timestamp;
            _packet.Ssrc = _ssrc;
            _packet.SetPayload(_pt, _payload);
            _packet.Pack(ref bytes, ref offset);

            // Assert
            Assert.AreNotEqual(offset, 0);
            Assert.AreEqual(offset, bytes.Length);
            CollectionAssert.AreEqual(bytes, _bytes);
        }

        #endregion Methods
    }
}
