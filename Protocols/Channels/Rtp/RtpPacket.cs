namespace Protocols.Channels.Rtp
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using Protocols.Utils;

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

        public byte PayloadType { get; set; }

        public ushort SequenceNumber { get; set; }

        public uint TimeStamp { get; set; }

        public uint Ssrc { get; set; }

        public IReadOnlyCollection<uint> CsrcList => _csrcList;

        public RtpExtension Extension { get; }

        public byte[] Payload { get; private set; }

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

        // TODO: Add calc extensions.
        public int GetByteLength() 
            => 
            PACKAGE_LENGTH + 
            CsrcCount * 4 + 
            Extension.GetByteLength() + 
            (Payload?.Length ?? 0);

        public bool TryUnpack(byte[] buffer, ref int offset)
        {
            var bufferSpace = buffer.Length - offset;

            if (bufferSpace < GetByteLength())
                return false;

            Version = BufferBits.GetByte(buffer, ref offset, 2);
            if (Version != DEFAULT_VERSION_VALUE)
                return false;

            HasPadding = BufferBits.GetBool(buffer, ref offset);
            HasExtension = BufferBits.GetBool(buffer, ref offset);
            CsrcCount = BufferBits.GetByte(buffer, ref offset, 4);
            Marker = BufferBits.GetBool(buffer, ref offset);
            PayloadType = BufferBits.GetByte(buffer, ref offset, 7);
            SequenceNumber = BufferBits.GetUInt16(buffer, ref offset);
            TimeStamp = BufferBits.GetUInt32(buffer, ref offset);

            if (bufferSpace < GetByteLength())
                return false;

            for (int i = 0; i < CsrcCount; i++)
                _csrcList.Add(BufferBits.GetUInt32(buffer, ref offset));

            if (HasExtension && !Extension.TryUnpack(buffer, ref offset))
                return false;

            Payload = BufferBits.GetBytes(buffer, ref offset, buffer.Length - offset);

            // TODO: Add support Padding.

            return true;
        }

        public void Pack(ref byte[] buffer, ref int offset)
        {
            BufferBits.Prepare(ref buffer, offset, GetByteLength() * 8);

            BufferBits.SetByte(Version, buffer, ref offset, 2);
            BufferBits.SetBool(HasPadding, buffer, ref offset);
            BufferBits.SetBool(HasExtension, buffer, ref offset);
            BufferBits.SetByte(CsrcCount, buffer, ref offset, 4);
            BufferBits.SetBool(Marker, buffer, ref offset);
            BufferBits.SetByte(PayloadType, buffer, ref offset, 7);
            BufferBits.SetUInt16(SequenceNumber, buffer, ref offset);
            BufferBits.SetUInt32(TimeStamp, buffer, ref offset);
            BufferBits.SetUInt32(Ssrc, buffer, ref offset);
            foreach (var csrc in CsrcList.Take(CsrcCount))
                BufferBits.SetUInt32(csrc, buffer, ref offset);

            if (HasExtension)
                Extension.Pack(ref buffer, ref offset);

            BufferBits.SetBytes(Payload, buffer, ref offset);
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
