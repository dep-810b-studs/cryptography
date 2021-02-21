using System;

namespace Cryptography.WorkingWithBits
{
    public class OpenedText
    {
        private uint _text;

        public OpenedText(uint text)
        {
            _text = text;
        }

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
        
        private void SetIBit(int bitNumber, uint bitValue)
        {
            if(bitNumber < 0)
                throw new ArgumentException(
                    $"The argument {nameof(bitNumber)} should be correct bit number (less or more then zero) but found {bitNumber}");
            
            if (bitValue is not 0 and not 1)
                throw new ArgumentException(
                    $"The argument {nameof(bitValue)} should be correct bit value (0 or 1) but found {bitValue}");
            
            if(GetIBit(bitNumber) == bitValue)
                return;

            _text = (~((uint)1 << bitNumber) & _text) | bitValue << bitNumber;
        }

        public void SwapBits(int firstBitNumber, int secondBitNumber)
        {
            var firstBit = GetIBit(firstBitNumber);
            var secondBit = GetIBit(secondBitNumber);
            
            if(firstBit == secondBit)
                return;
            
            SetIBit(secondBitNumber, firstBit);
            SetIBit(firstBitNumber, secondBit);
        }
   }
}