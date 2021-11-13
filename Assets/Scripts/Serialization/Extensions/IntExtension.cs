using System;

namespace Serialization.Extensions
{
    public static class IntExtension
    {
        public static byte[] GetBytes(this int value)
        {
            return BitConverter.GetBytes(value);
        }
        public static int GetValue(byte[] data)
        {
            return BitConverter.ToInt32(data, 0);
        }
        public static int GetValue(byte[] data, ref int offset)
        {
            int result = BitConverter.ToInt32(data, offset);
            offset += 4;
            return result;
        }
    }
}