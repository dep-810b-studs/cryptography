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
   }
}