using System;

namespace Cryptography.Algorithms
{
    public static class SystemExtensions
    {
        public static byte[] ToByteArray(this ulong value)
        {
                var size = 8;
        
                var result = new byte[size];
        
                for (var i = 0; i < size; i++)
                {
                    var bitOffset = (size - (i + 1)) * 8;
                    result[i] = (byte)((value & ((ulong)0xFF << bitOffset)) >> bitOffset);
                }
        
                return result;
        }
        
        public static ulong ToUInt64(this byte[] data)
        {
            var requiredSize = 8;

            if (data.Length != requiredSize)
            {
                throw new ArgumentException($"The byte-array \"{nameof(data)}\" must be exactly {requiredSize} bytes.");
            }

            var result = 0ul;

            for (var i = 0; i < requiredSize; i++)
            {
                result |= ((ulong)data[i] << ((requiredSize - (i + 1)) * 8));
            }

            return result;
        }
    }
}