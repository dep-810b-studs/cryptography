namespace Cryptography.Algorithms.Symmetric.Padding
{
    public class PaddingInfo
    {
        public PaddingInfo(int countBytesPadded)
        {
            CountBytesPadded = countBytesPadded;
        }

        public int CountBytesPadded { get; set; }
    }
}