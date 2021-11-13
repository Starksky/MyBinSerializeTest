using System.Text;

namespace Serialization.Extensions
{
    public static class StringExtension
    {
        public static byte[] GetBytes(this string value)
        {
            return new UTF8Encoding(true).GetBytes(value);
        }
        public static string GetValue(byte[] data)
        {
            return new UTF8Encoding(true).GetString(data);
        }
        public static string GetValue(byte[] data, ref int offset, int count)
        {
            string result = new UTF8Encoding(true).GetString(data, offset, count);
            offset += count;
            return result;
        }
    }
}