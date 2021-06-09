namespace Protocols.Channels
{
    public interface IPacket
    {
        void Pack(ref byte[] buffer, ref int offset);

        void Unpack(byte[] buffer, ref int offset, int count);
    }
}
