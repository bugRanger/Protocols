namespace Protocols
{
    using System;

    public interface IPacket
    {
        #region Methods

        bool TryUnpack(byte[] buffer, ref int offset);

        void Pack(ref byte[] buffer, ref int offset);

        ArraySegment<byte> Pack();

        #endregion Methods
    }
}
