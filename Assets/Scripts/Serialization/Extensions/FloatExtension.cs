using System;

namespace Serialization.Extensions
{
    public static class FloatExtension
    {
        public static byte[] GetBytes(this float value)
        {
            return BitConverter.GetBytes(value);
        }
        public static float GetValue(byte[] data)
        {
            return BitConverter.ToSingle(data, 0);
        }
        public static float GetValue(byte[] data, ref int offset)
        {
            float result = BitConverter.ToSingle(data, offset);
            offset += 4;
            return result;
        }
    }
}