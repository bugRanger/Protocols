namespace Protocols.Tests
{
    using NUnit.Framework;

    using Protocols.Utils;

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
            Assert.AreEqual(buffer[0], 253);
            Assert.AreEqual(buffer[1], 3);
        }
    }
}