using System;
using System.Linq;

namespace Cryptography.Arithmetic.WorkingWithBits;

public class OpenText
{
    private const int P = 32;

    private readonly int[] _multiplyDeBruijnBitPosition2 =
    {
        0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8,
        31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
    };

    public OpenText(uint text)
    {
        Value = text;
    }

    public uint Value { get; private set; }

    public uint this[int bitNumber]
    {
        get => GetIBit(bitNumber);
        set => SetIBit(bitNumber, value);
    }

    public uint Length => GetNumberLenInBits();

    private uint GetIBit(int bitNumber)
    {
        AssertBitNumberCorrect(bitNumber);
        return (Value >> bitNumber) & 1;
    }

    private OpenText SetIBit(int bitNumber, uint bitValue)
    {
        AssertBitNumberCorrect(bitNumber);

        if (bitValue is not 0 and not 1)
            throw new ArgumentException(
                $"The argument {nameof(bitValue)} should be correct bit value (0 or 1) but found {bitValue}");

        if (GetIBit(bitNumber) == bitValue)
            return this;

        Value = (~((uint)1 << bitNumber) & Value) | (bitValue << bitNumber);
        return this;
    }

    public OpenText SwapBits(int firstBitNumber, int secondBitNumber)
    {
        var firstBit = GetIBit(firstBitNumber);
        var secondBit = GetIBit(secondBitNumber);

        if (firstBit == secondBit)
            return this;

        SetIBit(secondBitNumber, firstBit);
        SetIBit(firstBitNumber, secondBit);

        return this;
    }

    public OpenText ResetToZeroLowOrderBits(int countLowerBits)
    {
        Value = (Value >> countLowerBits) << countLowerBits;
        return this;
    }

    public OpenText ReplaceBytesByPermutations(byte[] permutations)
    {
        if (permutations.Any(item => item > 3))
            throw new ArgumentException("Permutations table contains no valid elements.");

        uint mask = 0b11111111;

        Value = ((Value & mask) << (permutations[0] * 8)) |
                (((Value & (mask << 8)) >> 8) << (permutations[1] * 8)) |
                (((Value & (mask << 16)) >> 16) << (permutations[2] * 8)) |
                (((Value & (mask << 24)) >> 24) << (permutations[3] * 8));

        return this;
    }

    public OpenText CyclicShift(int shift, ShiftDirection direction)
    {
        Value = direction switch
        {
            ShiftDirection.Left => (Value << shift) | (Value >> (P - shift)),
            ShiftDirection.Right => (Value >> shift) | (Value << (P - shift))
        };

        return this;
    }

    public int GetDegreeOfTwoThatNeighborsOfNumber()
    {
        return (int)GetNumberLenInBits() - 1;
    }

    public int FindMaxTwoDegreeThatDivisibleByNumber()
    {
        return _multiplyDeBruijnBitPosition2[(((~Value + 1) & Value) * 0x077CB531U) >> 27];
    }

    private uint GetNumberLenInBits()
    {
        uint countBits = 0;
        var numberCopy = Value;

        while (numberCopy > 0)
        {
            numberCopy >>= 1;
            countBits++;
        }

        return countBits;
    }

    #region Utils

    private void AssertBitNumberCorrect(int bitNumber)
    {
        if (bitNumber is < 0 or > P)
            throw new ArgumentException(
                $"The argument {nameof(bitNumber)} should be correct bit number (equal or more then zero and less then {P}) but found {bitNumber}");
    }

    #endregion

    #region Implicits

    public static implicit operator OpenText(uint number)
    {
        return new(number);
    }

    #endregion

    #region Bitwise Operators Overriding

    public static OpenText operator <<(OpenText number, int shift)
    {
        return number.Value << shift;
    }

    public static OpenText operator ^(OpenText firstNumber, OpenText secondNumber)
    {
        return firstNumber.Value ^ secondNumber.Value;
    }

    #endregion
}