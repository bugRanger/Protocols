namespace Protocols.Channels.Rtp
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using Framework.Common;


    /*
         The format of an RTP packet:

         0                   1                   2                   3
         0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
        +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        |V=2|P|X|  CC   |M|     PT      |       sequence number         |
        +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        |                           timestamp                           |
        +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        |           synchronization source (SSRC) identifier            |
        +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
        |            contributing source (CSRC) identifiers             |
        |                            ....                               |
        +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        |                   RTP extension (OPTIONAL)                    |
        +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        |                          payload  ...                         |
        |                               +-------------------------------+
        |                               | RTP padding   | RTP pad count |
        +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    */
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

        public bool HasPadding { get; set; }

        public bool HasExtension { get; set; }

        public byte CsrcCount { get; protected set; }

        public bool Marker { get; set; }

        public byte PayloadType { get; private set; }

        public ushort SequenceNumber { get; set; }

        public uint TimeStamp { get; set; }

        public uint Ssrc { get; set; }

        public IReadOnlyCollection<uint> CsrcList => _csrcList;

        public RtpExtension Extension { get; }

        public ArraySegment<byte> Payload { get; private set; }

        public byte PaddingCount { get; private set; }

        #endregion Properties

        #region Constructors

        public RtpPacket()
        {
            _csrcList = new List<uint>();
            Extension = new RtpExtension();
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

        public int GetByteLength() 
            => 
            PACKAGE_LENGTH + 
            CsrcCount * 4 + 
            (HasPadding ? PaddingCount : 0) +
            (HasExtension ? Extension.GetByteLength() : 0) + 
            Payload.Count;

        public void SetPayload(byte payloadType, ArraySegment<byte> payload, byte padding = 0) 
        {
            PayloadType = payloadType;
            Payload = payload;
            PaddingCount = padding;
        }

        public void Unpack(byte[] buffer, ref int offset, int count)
        {
            if (count < GetByteLength())
                throw new ArgumentOutOfRangeException(nameof(count));

            var bitOffset = 0;
            CsrcCount = BufferBits.GetByte(buffer, offset, ref bitOffset, 4);
            HasExtension = BufferBits.GetBool(buffer, offset, ref bitOffset);
            HasPadding = BufferBits.GetBool(buffer, offset, ref bitOffset);
            Version = BufferBits.GetByte(buffer, ref offset, ref bitOffset, 2);
            if (Version != DEFAULT_VERSION_VALUE)
                throw new ArgumentException(nameof(Version));

            bitOffset = 0;
            PayloadType = BufferBits.GetByte(buffer, offset, ref bitOffset, 7);
            Marker = BufferBits.GetBool(buffer, ref offset, ref bitOffset);

            SequenceNumber = BufferBits.GetUInt16(buffer, ref offset);
            TimeStamp = BufferBits.GetUInt32(buffer, ref offset);
            Ssrc = BufferBits.GetUInt32(buffer, ref offset);

            if (count < GetByteLength())
                throw new ArgumentOutOfRangeException(nameof(count));

            for (int i = 0; i < CsrcCount; i++)
                _csrcList.Add(BufferBits.GetUInt32(buffer, ref offset));

            if (HasExtension)
            {
                Extension.Unpack(buffer, ref offset, buffer.Length);
            }

            var payloadCount = buffer.Length - offset;
            if (HasPadding)
            {
                var remain = buffer.Length - 1;
                PaddingCount = BufferBits.GetByte(buffer, ref remain);
                if (PaddingCount > 0)
                    payloadCount -= PaddingCount;
            }

            Payload = BufferBits.GetBytes(buffer, ref offset, payloadCount);

            if (HasPadding)
                offset += PaddingCount;
        }

        public void Pack(ref byte[] buffer, ref int offset)
        {
            BufferBits.Prepare(ref buffer, offset, GetByteLength());

            var bitOffset = 0;
            BufferBits.SetByte(CsrcCount, buffer, offset, ref bitOffset, 4);
            BufferBits.SetBool(HasExtension, buffer, offset, ref bitOffset);
            BufferBits.SetBool(HasPadding, buffer, offset, ref bitOffset);
            BufferBits.SetByte(Version, buffer, ref offset, ref bitOffset, 2);
            
            bitOffset = 0;
            BufferBits.SetByte(PayloadType, buffer, offset, ref bitOffset, 7);
            BufferBits.SetBool(Marker, buffer, ref offset, ref bitOffset);

            BufferBits.SetUInt16(SequenceNumber, buffer, ref offset);
            BufferBits.SetUInt32(TimeStamp, buffer, ref offset);
            BufferBits.SetUInt32(Ssrc, buffer, ref offset);
            foreach (var csrc in CsrcList.Take(CsrcCount))
                BufferBits.SetUInt32(csrc, buffer, ref offset);

            if (HasExtension)
                Extension.Pack(ref buffer, ref offset);
            
            if (Payload.Count > 0)
                BufferBits.SetBytes(Payload.ToArray(), buffer, ref offset);

            if (HasPadding && PaddingCount > 0)
            {
                offset += PaddingCount - 1;
                BufferBits.SetByte(PaddingCount, buffer, ref offset);
            }
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
