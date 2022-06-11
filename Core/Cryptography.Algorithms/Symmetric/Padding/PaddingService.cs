using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cryptography.Algorithms.Symmetric.Padding
{
    public interface IPaddingService
    {
        (byte[] FilledData, byte [] PaddingInfo) FillEmptyBytes(byte[] data, int blockSizeInBytes);
        void RemovePaddedBytes(List<byte[]> data, int blockSizeInBytes);
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

        public void RemovePaddedBytes(List<byte[]> data, int blockSizeInBytes)
        {
            var countPaddedBytes = data.Last().First();
            data.Remove(data.Last());
            var paddedBlock = data.Last();
            var blockWithOutPaddingLength = blockSizeInBytes - countPaddedBytes;
            var blockWithOutPadding = paddedBlock[..blockWithOutPaddingLength];
            data.Remove(paddedBlock);
            data.Add(blockWithOutPadding);
        }
    }
}