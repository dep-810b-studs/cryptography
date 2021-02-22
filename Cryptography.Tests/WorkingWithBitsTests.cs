using Cryptography.WorkingWithBits;
using Xunit;

namespace Cryptography.Tests
{
    public class WorkingWithBitsTests
    {
        [Theory]
        [InlineData(1,0,1)]
        [InlineData(0,0,0)]
        [InlineData(0,10,0)]
        [InlineData(0b1010,3,1)]
        [InlineData(0b1010,2,0)]
        public void IndexerShouldReturnCorrectBit(uint number, int bitNumber, int expectedBit)
        { 
            //arrange
            var text = new OpenText(number);
            //act
            var actualBit = (int) text[bitNumber];
            //assert
            Assert.Equal(expectedBit, actualBit);
        }
        
        [Theory]
        [InlineData(0b1010,3,0)]
        [InlineData(0b1010,2,1)]
        [InlineData(0b10101,3,0)]
        [InlineData(0b10101,3,1)]
        [InlineData(0b10101,0,0)]
        [InlineData(0b11111,0,0)]
        public void IndexerShouldCorrectlySetBit(uint number, int bitNumber, uint bitValue)
        { 
            //arrange
            var text = new OpenText(number);
            //act
            text[bitNumber] = bitValue;
            var actualBitValue = (int) text[bitNumber];
            //assert
            Assert.Equal((int)bitValue, actualBitValue);
        }
        
        
        [Theory]
        [InlineData(0b1010,0,1)]
        [InlineData(0b1010,1,0)]
        [InlineData(0b10101,3,0)]
        [InlineData(0b10101,3,1)]
        public void BitsShouldBeCorrectSwapped(uint number, int firstBitNumber, int secondBitNumber)
        { 
            //arrange
            var text = new OpenText(number);
            var expectedFirstBit = text[secondBitNumber]; 
            var expectedSecondBit = text[firstBitNumber]; 
            //act
            text.SwapBits(firstBitNumber, secondBitNumber);            
            var actualFirstBit = text[firstBitNumber];
            var actualSecondBit = text[secondBitNumber];
            //assert
            Assert.Equal(expectedFirstBit, actualFirstBit);
            Assert.Equal(expectedSecondBit, actualSecondBit);
        }
        
        [Theory]
        [InlineData(0b1010,3,0b1000)]
        [InlineData(0b1010,2,0b1000)]
        [InlineData(0b111111,6,0b0)]
        public void BitsShouldBeCorrectResetToZero(uint number, int countLowerBits, uint expectedResult)
        { 
            //arrange
            var text = new OpenText(number);
            //act
            text.ResetToZeroLowOrderBits(countLowerBits);
            //assert
            Assert.Equal(expectedResult, text.Value);
        }
    }
}