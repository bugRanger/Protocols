namespace Protocols.Extensions
{
    public static class PacketExtension
    {
        public static byte[] PackWith(this IPacket first, params IPacket[] seconds) 
        {
            byte[] buffer = new byte[0];
            int offset = 0;

            first.Pack(ref buffer, ref offset);
            foreach (var item in seconds)
                item.Pack(ref buffer, ref offset);

            return buffer;
        }
    }
}
