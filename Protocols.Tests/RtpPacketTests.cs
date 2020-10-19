namespace Protocols.Tests
{
    using System;

    using NUnit.Framework
        ;
    using Protocols.Channels.Rtp;

    [TestFixture]
    public class RtpPacketTests
    {
        #region Fields

        private RtpPacket _packet;

        #endregion Fields

        #region Methods

        [SetUp]
        public void SetUp() 
        {
            _packet = new RtpPacket();
        }

        [Test]
        public void UnpackTest()
        {
            //// Arrage
            //var offset = 0;
            //byte[] bytes = Encoding.ASCII.GetBytes(message);

            //// Act
            //bool result = _packet.TryUnpack(bytes, ref offset);

            //// TODO Add more asserts.
            //// Assert
            //Assert.IsTrue(result);
            //Assert.AreEqual(offset, bytes.Length);
            //Assert.AreEqual(_packet.?, ?);
        }

        [Test]
        public void UnpackWithOffsetTest()
        {
            // Arrage
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
            _packet.Pack(ref bytes, ref offset);

            // Assert
            Assert.AreNotEqual(offset, 0);
            Assert.AreEqual(offset, bytes.Length * 8);
        }

        #endregion Methods
    }
}
