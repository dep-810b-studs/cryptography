using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptography.WorkingWithBits
{
    public class OpenText
    {
        private uint _text;

        public OpenText(uint text)
        {
            _text = text;
        }

        public uint Value => _text;
        
        public uint this[int bitNumber]
        {
            get => GetIBit(bitNumber);
            set => SetIBit(bitNumber, value);
        }

        private uint GetIBit(int bitNumber)
        {
            if(bitNumber < 0)
                throw new ArgumentException(
                    $"The argument {nameof(bitNumber)} should be correct bit number (less or more then zero) but found {bitNumber}");
            
            return _text >> bitNumber & 1;
        }
        
        private OpenText SetIBit(int bitNumber, uint bitValue)
        {
            if(bitNumber < 0)
                throw new ArgumentException(
                    $"The argument {nameof(bitNumber)} should be correct bit number (less or more then zero) but found {bitNumber}");
            
            if (bitValue is not 0 and not 1)
                throw new ArgumentException(
                    $"The argument {nameof(bitValue)} should be correct bit value (0 or 1) but found {bitValue}");
            
            if(GetIBit(bitNumber) == bitValue)
                return this;

            _text = (~((uint)1 << bitNumber) & _text) | bitValue << bitNumber;
            return this;
        }

        public OpenText SwapBits(int firstBitNumber, int secondBitNumber)
        {
            var firstBit = GetIBit(firstBitNumber);
            var secondBit = GetIBit(secondBitNumber);
            
            if(firstBit == secondBit)
                return this;
            
            SetIBit(secondBitNumber, firstBit);
            SetIBit(firstBitNumber, secondBit);

            return this;
        }

        public OpenText ResetToZeroLowOrderBits(int countLowerBits)
        {
           _text = _text >> countLowerBits << countLowerBits;
           return this;
        }

        public OpenText ReplaceBitsByPermutations(byte[] permutations)
        {
            var textWithReplacedBits = new OpenText(0);

            if (permutations.Any(item => item > 32 ))
                throw new ArgumentException("Permutations table contains no valid elements.");
            
            Parallel.For(0, permutations.Length, (i) =>
            {
                textWithReplacedBits[i] = this[permutations[i]];
            });

            _text = textWithReplacedBits.Value;
            return this;
        }

        public OpenText CyclicShift(int shift, ShiftDirection direction)
        {
            int p = 32;
            
            _text = direction switch
            {
                ShiftDirection.Left => (uint) (((_text << shift) & ~(-1 << p)) | (((-1 << p - shift) & _text) >> (p-shift))),
                ShiftDirection.Right => (uint) ((_text >> shift) | ((~(-1 << shift) & shift) << 32-shift))
            };

            return this;
        }

        public int GetDegreeOfTwoThatNeighborsOfNumber()
        {
            var result = -1;
            uint textCopy = _text;

            while (textCopy > 0)
            {
                textCopy >>= 1;
                result++;
            }

            return result;
        }
        
        public int FindMaxTwoDegreeThatDivisibleByNumber()
        {
            return _multiplyDeBruijnBitPosition2[(((~_text + 1) & _text) * 0x077CB531U) >> 27];
        }

        private readonly int[] _multiplyDeBruijnBitPosition2 = 
        {
            0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8,
            31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
        };
    }
}