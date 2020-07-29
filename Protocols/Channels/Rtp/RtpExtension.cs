namespace Protocols.Channels.Rtp
{
    using System;
    using System.Collections.Generic;

    using Framework.Common;


    public class RtpExtension : IPacket
    {
        #region Fields

        private readonly List<uint> _extensionList;

        #endregion Fields

        #region Properties

        public ushort Profile { get; set; }

        public ushort Length { get; private set; }

        public IReadOnlyCollection<uint> Extension { get; private set; }

        #endregion Properties

        #region Constructors

        internal RtpExtension() 
        {
            _extensionList = new List<uint>();
        }

        #endregion Constructors

        #region Methods

        public int GetByteLength() => 4 + Length;

        public bool TryAddExtension(uint extension)
        {
            _extensionList.Add(extension);
            Length++;

            return true;
        }

        public void RemoveExtension(uint extension)
        {
            _extensionList.Remove(extension);
            Length--;
        }

        public bool TryUnpack(byte[] buffer, ref int offset)
        {
            var bufferSpace = buffer.Length - offset;

            if (bufferSpace < GetByteLength())
                return false;

            Profile = BufferBits.GetUInt16(buffer, ref offset);
            Length = BufferBits.GetUInt16(buffer, ref offset);
            for (int i = 0; i < Length; i++)
                _extensionList.Add(BufferBits.GetUInt32(buffer, ref offset));

            offset += Extension.Count * 8;
            return true;
        }

        public void Pack(ref byte[] buffer, ref int offset)
        {
            BufferBits.SetUInt16(Profile, buffer, ref offset);
            BufferBits.SetUInt16(Length, buffer, ref offset);
            foreach (var extension in Extension)
                BufferBits.SetUInt32(extension, buffer, ref offset);
        }

        public ArraySegment<byte> Pack()
        {
            var buffer = new byte[GetByteLength()];
            var offset = 0;
            Pack(ref buffer, ref offset);
            return new ArraySegment<byte>(buffer, 0, offset);
        }

        #endregion Methods
    }
}
