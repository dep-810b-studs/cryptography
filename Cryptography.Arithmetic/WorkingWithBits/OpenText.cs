using System;
using System.Linq;

namespace Cryptography.Arithmetic.WorkingWithBits
{
    public class OpenText
    {
        private uint _text;
        private const int P = 32;

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
            AssertBitNumberCorrect(bitNumber);
            return _text >> bitNumber & 1;
        }
        
        private OpenText SetIBit(int bitNumber, uint bitValue)
        {
            AssertBitNumberCorrect(bitNumber);

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

        public OpenText ReplaceBytesByPermutations(byte[] permutations)
        {
            if (permutations.Any(item => item > 3 ))
                throw new ArgumentException("Permutations table contains no valid elements.");

            uint mask = 0b11111111;

            _text = (_text & mask) << permutations[0] * 8 |
                    ((_text & mask << 8) >> 8) << permutations[1] * 8 |
                    ((_text & mask << 16) >> 16) << permutations[2] * 8 |
                    ((_text & mask << 24) >> 24) << permutations[3] * 8;
            
            return this;
        }

        public OpenText CyclicShift(int shift, ShiftDirection direction)
        {
            _text = direction switch
            {
                ShiftDirection.Left => _text << shift | _text >> (P - shift),
                
                ShiftDirection.Right => _text >> shift | _text << (P - shift)
            };

            return this;
        }

        public int GetDegreeOfTwoThatNeighborsOfNumber()
        {
            var result = 0;
            uint textCopy = _text;

            while (textCopy > 0)
            {
                textCopy >>= 1;
                result++;
            }

            return result - 1;
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

        #region Utils

        private void AssertBitNumberCorrect(int bitNumber)
        {
            if(bitNumber is < 0 or > P)
                throw new ArgumentException(
                    $"The argument {nameof(bitNumber)} should be correct bit number (equal or more then zero and less then {P}) but found {bitNumber}");
        }

        #endregion
    }
}