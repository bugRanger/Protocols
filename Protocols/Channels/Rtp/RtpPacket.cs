namespace Protocols.Channels.Rtp
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using Protocols.Utils;

    public class RtpPacket : IPacket
    {
        #region Constants

        private const byte PACKAGE_LENGTH = 12;

        private const byte CSRS_LIST_MAX_COUNT = 15;

        private const byte DEFAULT_VERSION_VALUE = 2;

        #endregion Constants

        #region Fields

        private List<uint> _csrcList;

        #endregion Fields

        #region Properties

        public byte Version { get; set; }

        public bool Padding { get; set; }

        public bool Extension { get; set; }

        public int CsrcCount { get; protected set; }

        public bool Marker { get; set; }

        public byte PayloadType { get; set; }

        public ushort SequenceNumber { get; set; }

        public uint TimeStamp { get; set; }

        public uint Ssrc { get; set; }

        public IReadOnlyCollection<uint> CsrcList => _csrcList;

        #endregion Properties

        #region Constructors

        public RtpPacket() 
        {
            _csrcList = new List<uint>();
        }

        #endregion Constructors

        #region Methods

        public bool TryAddCsrc(uint csrc) 
        {
            if (_csrcList.Count == CSRS_LIST_MAX_COUNT)
                return false;

            _csrcList.Add(csrc);
            CsrcCount++;

            return true;
        }

        public void RemoveCsrc(uint csrc)
        {
            _csrcList.Remove(csrc);
            CsrcCount--;
        }

        public int GetByteLength() => PACKAGE_LENGTH + (CsrcCount * 4);

        public bool TryUnpack(byte[] buffer, ref int offset, out IPacket packet)
        {
            packet = null;
            var bufferSpace = buffer.Length - offset;

            if (bufferSpace < GetByteLength())
                return false;

            Version = BufferBits.GetByte(buffer, ref offset, 2);
            Padding = BufferBits.GetBool(buffer, ref offset);
            Extension = BufferBits.GetBool(buffer, ref offset);
            CsrcCount = BufferBits.GetByte(buffer, ref offset, 4);
            Marker = BufferBits.GetBool(buffer, ref offset);
            PayloadType = BufferBits.GetByte(buffer, ref offset, 7);
            SequenceNumber = BufferBits.GetUInt16(buffer, ref offset, 16);
            TimeStamp = BufferBits.GetUInt32(buffer, ref offset, 32);

            if (bufferSpace < GetByteLength())
                return false;

            for (int i = 0; i < CsrcCount; i++)
                _csrcList.Add(BufferBits.GetUInt32(buffer, ref offset, 32));
            
            return true;
        }

        public void Pack(ref byte[] buffer, ref int offset)
        {
            BufferBits.Prepare(ref buffer, offset, GetByteLength() * 8);

            BufferBits.SetByte(buffer, ref offset, 2, Version);
            BufferBits.SetBool(buffer, ref offset, Padding);
            BufferBits.SetBool(buffer, ref offset, Extension);
            BufferBits.SetValue(buffer, ref offset, 4, CsrcCount);
            BufferBits.SetBool(buffer, ref offset, Marker);
            BufferBits.SetValue(buffer, ref offset, 7, PayloadType);
            BufferBits.SetValue(buffer, ref offset, 16, SequenceNumber);
            BufferBits.SetValue(buffer, ref offset, 32, TimeStamp);
            BufferBits.SetValue(buffer, ref offset, 32, Ssrc);
            foreach (var csrc in CsrcList.Take(CsrcCount))
                BufferBits.SetValue(buffer, ref offset, 32, csrc);
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
