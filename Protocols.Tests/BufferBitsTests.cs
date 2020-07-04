namespace Protocols.Tests
{
    using NUnit.Framework;

    using Protocols.Utils;

    [TestFixture]
    public class BufferBitsTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PrepareTest()
        {
            // Arrage
            var buffer = new byte[] { 1, 2, };
            var offset = 2;
            var count = 32;

            // Act
            BufferBits.Prepare(ref buffer, offset, count);

            // Assert
            Assert.AreEqual(buffer.Length, 5);
        }

        [Test]
        public void GetValueTest()
        {
            // Arrage
            var buffer = new byte[] { 1, 2, 3, 4, 5, 6 };
            var offset = 2;
            var count = 12;

            // Act
            var result = BufferBits.GetValue(buffer, ref offset, count);

            // Assert
            Assert.AreEqual(offset, 14);
            Assert.AreEqual(result, 128);
        }

        [Test]
        public void SetValueTest()
        {
            // Arrage
            var buffer = new byte[] { 1, 2, 3, 4, 5, 6 };
            var offset = 2;
            var count = 8;
            var value = 255;

            // Act
            BufferBits.SetValue(buffer, ref offset, count, value);

            // Assert
            Assert.AreEqual(offset, 10);
            Assert.AreEqual(buffer[0], 253);
            Assert.AreEqual(buffer[1], 3);
            Assert.AreEqual(buffer[2], 3);
            Assert.AreEqual(buffer[3], 4);
            Assert.AreEqual(buffer[4], 5);
            Assert.AreEqual(buffer[5], 6);
        }

        [Test]
        public void GetValueOutOfRangeTest()
        {
            // Arrage
            var buffer = new byte[] { 1, 2, 3, 4, 5, 6 };
            var offset = 6 * 8;
            var count = 12;

            // Act
            var result = BufferBits.GetValue(buffer, ref offset, count);

            // Assert
            Assert.AreEqual(offset, 6 * 8);
            Assert.AreEqual(result, 0);
        }

        [Test]
        public void SetValueOutOfRangeTest()
        {
            // Arrage
            var buffer = new byte[] { 1, 2, 3, 4, 5, 6 };
            var offset = 6 * 8;
            var count = 8;
            var value = 255;

            // Act
            BufferBits.SetValue(buffer, ref offset, count, value);

            // Assert
            Assert.AreEqual(offset, 6 * 8);
            Assert.AreEqual(buffer[0], 1);
            Assert.AreEqual(buffer[1], 2);
            Assert.AreEqual(buffer[2], 3);
            Assert.AreEqual(buffer[3], 4);
            Assert.AreEqual(buffer[4], 5);
            Assert.AreEqual(buffer[5], 6);
        }

        [Test]
        public void GetBytesBitOffsetTest()
        {
            // Arrage
            var buffer = new byte[] { 1, 2, 255, 255, 255, 255 };
            var offset = 2;
            var length = 3;

            // Act
            var result = BufferBits.GetBytes(buffer, ref offset, length);

            // Assert
            Assert.AreEqual(offset, 26);
            Assert.AreEqual(result[0], 128);
            Assert.AreEqual(result[1], 192);
            Assert.AreEqual(result[2], 255);
        }

        [Test]
        public void SetBytesBitOffsetTest()
        {
            // Arrage
            var buffer = new byte[] { 1, 2, 3, 4, 5, 6 };
            var source = new byte[] { 3, 1, 2 };
            var offset = 2;
            var length = 3;

            // Act
            BufferBits.SetBytes(source, buffer, ref offset, length);

            // Assert
            Assert.AreEqual(offset, 26);
            Assert.AreEqual(buffer[0], 13);
            Assert.AreEqual(buffer[1], 4);
            Assert.AreEqual(buffer[2], 8);
        }
    }
}