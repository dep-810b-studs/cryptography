using System;
using System.Collections;
using System.Collections.Generic;
using Cryptography.Arithmetic.WorkingWithBits;
using Xunit;
using Xunit.Abstractions;

namespace Cryptography.Tests;

public class WorkingWithBitsTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public WorkingWithBitsTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(1, 0, 1)]
    [InlineData(0, 0, 0)]
    [InlineData(0, 10, 0)]
    [InlineData(0b1010, 3, 1)]
    [InlineData(0b1010, 2, 0)]
    public void IndexerShouldReturnCorrectBit(uint number, int bitNumber, int expectedBit)
    {
        //arrange
        var text = new OpenText(number);
        //act
        var actualBit = (int)text[bitNumber];
        //assert
        Assert.Equal(expectedBit, actualBit);
    }

    [Theory]
    [InlineData(0b1010, 3, 0)]
    [InlineData(0b1010, 2, 1)]
    [InlineData(0b10101, 3, 0)]
    [InlineData(0b10101, 3, 1)]
    [InlineData(0b10101, 0, 0)]
    [InlineData(0b11111, 0, 0)]
    public void IndexerShouldCorrectlySetBit(uint number, int bitNumber, uint bitValue)
    {
        //arrange
        var text = new OpenText(number);
        //act
        text[bitNumber] = bitValue;
        var actualBitValue = (int)text[bitNumber];
        //assert
        Assert.Equal((int)bitValue, actualBitValue);
    }


    [Theory]
    [InlineData(0b1010, 0, 1)]
    [InlineData(0b1010, 1, 0)]
    [InlineData(0b10101, 3, 0)]
    [InlineData(0b10101, 3, 1)]
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
    [InlineData(0b1010, 3, 0b1000)]
    [InlineData(0b1010, 2, 0b1000)]
    [InlineData(0b111111, 6, 0b0)]
    public void BitsShouldBeCorrectResetToZero(uint number, int countLowerBits, uint expectedResult)
    {
        //arrange
        var text = new OpenText(number);
        //act
        text.ResetToZeroLowOrderBits(countLowerBits);
        //assert
        Assert.Equal(expectedResult, text.Value);
    }

    [Theory]
    [ClassData(typeof(PermutationsTestsData))]
    public void BytesShouldBeCorrectReplacedByPermutations(uint number, uint expectedResult, byte[] permutations)
    {
        //arrange
        var text = new OpenText(number);
        //act
        text.ReplaceBytesByPermutations(permutations);
        _testOutputHelper.WriteLine(Convert.ToString(text.Value, 2));
        //assert
        Assert.Equal(expectedResult, text.Value);
    }

    [Theory]
    [InlineData(4)]
    public void ShouldBeThrownExceptionWhenIllegalItemInPermutationsTable(byte permutation)
    {
        //arrange
        var permutations = new[] { permutation };
        var text = new OpenText(0);
        //assert
        Assert.Throws(typeof(ArgumentException), () => text.ReplaceBytesByPermutations(permutations));
    }

    [Theory]
    [InlineData(24, 3)]
    [InlineData(28, 2)]
    [InlineData(64, 6)]
    [InlineData(63, 0)]
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
        1, ShiftDirection.Right)]
    [InlineData(
        0b10000000000000000000000000000000,
        0b00000000000000000000000000000001,
        1, ShiftDirection.Left)]
    [InlineData(
        0b11111111111111110000000000000000,
        0b11111111111111100000000000000001,
        1, ShiftDirection.Left)]
    [InlineData(
        0b11111111111111110000000000000000,
        0b01111111111111111000000000000000,
        1, ShiftDirection.Right)]
    [InlineData(
        0b11111111111111110000000000000000,
        0b00000000000000001111111111111111,
        16, ShiftDirection.Left)]
    [InlineData(
        0b11111111_11111111_00000000_00000000,
        0b00000000_00000000_11111111_11111111,
        16, ShiftDirection.Right)]
    public void CyclicShiftShouldCorrectWork(uint number, uint expectedResult, int shift, ShiftDirection shiftDirection)
    {
        //arrange
        var text = new OpenText(number);
        //act
        text.CyclicShift(shift, shiftDirection);
        //assert
        Assert.Equal(expectedResult, text.Value);
    }

    [Theory]
    [InlineData(50, 5)]
    [InlineData(70, 6)]
    public void ShouldReturnCorrectDegreeOfTwoThatNeighborsOfNumber(uint number, int expectedDegree)
    {
        //arrange
        var text = new OpenText(number);
        //act
        var actualDegree = text.GetDegreeOfTwoThatNeighborsOfNumber();
        //assert
        Assert.Equal(expectedDegree, actualDegree);
    }

    private class PermutationsTestsData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                0b10000000110000001110000011110000,
                0b11110000111000001100000010000000,
                new byte[] { 3, 2, 1, 0 }
            };
            yield return new object[]
            {
                0b100001001101111,
                0b110111101000010,
                new byte[] { 1, 0, 2, 3 }
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}