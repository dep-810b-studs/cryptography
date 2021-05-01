using System;
using System.Text;
using System.Text.Json;

namespace Cryptography.Algorithms.Symmetric.Padding
{
    public interface IPaddingService
    {
        (byte[] FilledData, byte [] PaddingInfo) FillEmptyBytes(byte[] data, int blockSize);
        PaddingInfo TryGetPaddingInfo(byte[] paddingInfoSource)
    }
    
    public class PaddingService : IPaddingService
    {
        public (byte[] FilledData, byte [] PaddingInfo) FillEmptyBytes(byte[] data, int blockSize)
        {
            var filledData = new byte[blockSize];

            for (int i = 0; i < data.Length; i++)
            {
                filledData[i] = data[i];
            }

            var countEmptyBytes = blockSize - data.Length;
            var paddingInfo = new PaddingInfo(countEmptyBytes);

            var paddingInfoInBytes = JsonSerializer.SerializeToUtf8Bytes(paddingInfo);
            return (filledData, paddingInfoInBytes);
        }

        public PaddingInfo TryGetPaddingInfo(byte[] paddingInfoSource)
        {
            var stringPaddingInfo = Encoding.UTF8.GetString(paddingInfoSource);
            var paddingInfo = JsonSerializer.Deserialize<PaddingInfo>(stringPaddingInfo);
            return paddingInfo;
        }
    }
}