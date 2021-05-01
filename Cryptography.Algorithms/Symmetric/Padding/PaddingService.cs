using System;
using System.Text;
using System.Text.Json;

namespace Cryptography.Algorithms.Symmetric.Padding
{
    public interface IPaddingService
    {
        (byte[] FilledData, byte [] PaddingInfo) FillEmptyBytes(byte[] data, int blockSizeInBytes);
        PaddingInfo TryGetPaddingInfo(byte[] paddingInfoSource);
    }
    
    public class PaddingService : IPaddingService
    {
        public (byte[] FilledData, byte [] PaddingInfo) FillEmptyBytes(byte[] data, int blockSizeInBytes)
        {
            var filledData = new byte[blockSizeInBytes];

            for (int i = 0; i < data.Length; i++)
            {
                filledData[i] = data[i];
            }

            var countEmptyBytes = blockSizeInBytes - data.Length;
            var paddingInfoInBytes = new byte[blockSizeInBytes];
            paddingInfoInBytes[0] = (byte)countEmptyBytes;
            return (filledData, paddingInfoInBytes);
        }

        public PaddingInfo TryGetPaddingInfo(byte[] paddingInfoSource)
        {
            var paddingInfo = new PaddingInfo(paddingInfoSource[0]);
            return paddingInfo;
        }
    }
}