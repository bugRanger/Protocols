namespace Protocols.Utils
{
    using System;

    public static class BufferBits
    {
        #region Methods

        public static void Prepare(ref byte[] buffer, int offset, int count)
        {
            int bytes = count / 8 + (offset % 8 > 0 ? 1 : 0);
            int total = buffer.Length - (offset / 8);
            if (total < bytes)
                Array.Resize(ref buffer, buffer.Length + (bytes - total));
        }

        public static bool GetBool(byte[] buffer, ref int offset)
            => Convert.ToBoolean(GetValue(buffer, ref offset, 1));

        public static byte GetByte(byte[] buffer, ref int offset, int count)
            => Convert.ToByte(GetValue(buffer, ref offset, count));

        public static short GetInt16(byte[] buffer, ref int offset, int count)
            => Convert.ToInt16(GetValue(buffer, ref offset, count));
        public static ushort GetUInt16(byte[] buffer, ref int offset, int count)
            => Convert.ToUInt16(GetValue(buffer, ref offset, count));

        public static int GetInt32(byte[] buffer, ref int offset, int count)
            => Convert.ToInt32(GetValue(buffer, ref offset, count));
        public static uint GetUInt32(byte[] buffer, ref int offset, int count)
            => Convert.ToUInt32(GetValue(buffer, ref offset, count));

        public static long GetInt64(byte[] buffer, ref int offset, int count)
            => GetValue(buffer, ref offset, count);

        public static ulong GetUInt64(byte[] buffer, ref int offset, int count)
            => Convert.ToUInt64(GetValue(buffer, ref offset, count));


        public static bool GetBool(byte[] buffer, int offset)
            => GetBool(buffer, ref offset);

        public static byte GetByte(byte[] buffer, int offset, int count)
            => GetByte(buffer, ref offset, count);

        public static short GetInt16(byte[] buffer, int offset, int count)
            => GetInt16(buffer, ref offset, count);

        public static int GetInt32(byte[] buffer, int offset, int count)
            => GetInt32(buffer, ref offset, count);

        public static long GetInt64(byte[] buffer, int offset, int count)
            => GetInt64(buffer, ref offset, count);

        public static ushort GetUInt16(byte[] buffer, int offset, int count)
            => GetUInt16(buffer, ref offset, count);

        public static uint GetUInt32(byte[] buffer, int offset, int count)
            => GetUInt32(buffer, ref offset, count);

        public static ulong GetUInt64(byte[] buffer, int offset, int count)
            => GetUInt64(buffer, ref offset, count);


        public static void SetBool(byte[] buffer, ref int offset, bool value)
            => SetValue(buffer, ref offset, 1, Convert.ToByte(value));

        public static void SetByte(byte[] buffer, ref int offset, int count, byte value)
            => SetValue(buffer, ref offset, count, value);

        public static void SetInt16(byte[] buffer, ref int offset, int count, short value)
            => SetValue(buffer, ref offset, count, value);

        public static void SetInt32(byte[] buffer, ref int offset, int count, int value)
            => SetValue(buffer, ref offset, count, value);

        public static void SetInt64(byte[] buffer, ref int offset, int count, long value)
            => SetValue(buffer, ref offset, count, value);


        public static long GetValue(byte[] buffer, ref int offset, int count)
        {
            int bytes = count / 8 + (offset % 8 > 0 ? 1 : 0);
            long mask = long.MaxValue >> (63 - count);
            long value = 0;

            unsafe
            {
                long* pointerToValue = &value;
                fixed (byte* pointerToBuffer = &buffer[offset / 8]) 
                {
                    Buffer.MemoryCopy(pointerToBuffer, pointerToValue, bytes, bytes);
                }
            }

            value = (value >> offset) & mask;
            offset += count;

            return value;
        }

        public static void SetValue(byte[] buffer, ref int offset, int count, long value)
        {
            int bytes = count / 8 + (offset % 8 > 0 ? 1 : 0);
            long mask = long.MaxValue >> (sizeof(long) * 8 - count - 1);
            value = mask & value;

            unsafe
            {
                long bufferValue;
                long* pointerToBufferValue = &bufferValue;
                fixed (byte* pointerToBuffer = &buffer[offset / 8])
                {
                    Buffer.MemoryCopy(pointerToBuffer, pointerToBufferValue, bytes, bytes);
                    bufferValue = (bufferValue & ~(mask << offset)) | (value << offset);
                    Buffer.MemoryCopy(pointerToBufferValue, pointerToBuffer, bytes, bytes);
                }
            }

            offset += count;
        }

        #endregion Methods

    }
}
