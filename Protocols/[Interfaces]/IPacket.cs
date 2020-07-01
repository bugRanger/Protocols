namespace Protocols
{
    using System;

    public interface IPacket
    {
        #region Methods

        bool TryUnpack(byte[] buffer, ref int offset, int count, out IPacket packet);

        void Pack(ref byte[] buffer, ref int offset, int count);

        ArraySegment<byte> Pack();

        #endregion Methods
    }
}
