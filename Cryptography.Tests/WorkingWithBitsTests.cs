using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cryptography.WorkingWithBits;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
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
        
        private class PermutationsTestsData: IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { 0b1010,0b0101,new byte[]{3,2,1,0}};
                yield return new object[] { 0b11110000,0b00001111,new byte[]{7,6,5,4,3,2,1,0}};
                yield return new object[] { 0b10101010,0b01010101,new byte[]{7,6,5,4,3,2,1,0}};
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        
        [Theory]
        [ClassData(typeof(PermutationsTestsData))]
        public void BitsShouldBeCorrectReplacedByPermutations(uint number, uint expectedResult, byte[] permutations)
        { 
            //arrange
            var text = new OpenText(number);
            //act
            text.ReplaceBitsByPermutations(permutations);
            //assert
            Assert.Equal(expectedResult, text.Value);
        }
        
        [Theory]
        [InlineData(33)]
        public void ShouldBeThrownExceptionWhenIllegalItemInPermutationsTable(byte permutation)
        { 
            //arrange
            var permutations = new [] {permutation};
            var text = new OpenText(0);
            //assert
            Assert.Throws(typeof(ArgumentException),()=> text.ReplaceBitsByPermutations(permutations));
        }
        
        [Theory]
        [InlineData(24,3)]
        [InlineData(28,2)]
        [InlineData(64,6)]
        [InlineData(63,0)]
        public void MaxTwoDegreeThatDivisibleByNumberShouldBeCorrectFounded(uint number, byte expectedDegree)
        { 
            //arrange
            var text = new OpenText(number);
            //act
            var actualDegree = text.FindMaxTwoDegreeThatDivisibleByNumber();
            //assert
            Assert.Equal(expectedDegree, actualDegree);
        }

        [Theory]
        [InlineData(
            0b00000000000000000000000000000001,
            0b10000000000000000000000000000000,
            1,ShiftDirection.Right)]
        [InlineData(
            0b10000000000000000000000000000000,
            0b00000000000000000000000000000001,
            1,ShiftDirection.Left)]
        public void CyclicShiftShouldCorrectWork(uint number, uint expectedResult,int shift, ShiftDirection shiftDirection)
        {
            //arrange
            var text = new OpenText(number);
            //act
            text.CyclicShift(shift, shiftDirection);
            //assert
            Assert.Equal(expectedResult,text.Value);
        }

    }
}