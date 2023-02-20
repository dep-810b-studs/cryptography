using System;
using System.Linq;
using Cryptography.Arithmetic.WorkingWithBits;

namespace Cryptography.Arithmetic;

public class BinaryPolynomial
{
    public BinaryPolynomial(uint value)
    {
        Value = value;
    }

    public uint Value { get; }

    public static BinaryPolynomial Zero => new(0);
    public static BinaryPolynomial One => new(1);

    public override string ToString()
    {
        return ToPotentialForm(Value, 32);
    }

    public BinaryPolynomial Copy()
    {
        return new BinaryPolynomial(Value);
    }

    public static (BinaryPolynomial d, BinaryPolynomial x, BinaryPolynomial y) ExtendedEuclideanAlgorithm(
        BinaryPolynomial a, BinaryPolynomial b)
    {
        if (b == Zero) return (a, 1, 0);

        BinaryPolynomial x1 = One, y1 = Zero, x2 = Zero, y2 = One;

        while (b > 0)
        {
            var q = a / b;

            BinaryPolynomial t;

            t = b.Copy();
            b = a % b;
            a = t.Copy();

            t = x2.Copy();
            x2 = x1 - q * x2;
            x1 = t.Copy();

            t = y2.Copy();
            y2 = y1 - q * y2;
            y1 = t.Copy();
        }

        return (a, x1, y1);
    }

    public override bool Equals(object? obj)
    {
        if (obj is BinaryPolynomial binaryPolynomial)
            return Value == binaryPolynomial.Value;

        return false;
    }

    #region Arithmetic Operations

    public static BinaryPolynomial operator +(BinaryPolynomial firstPolynomial, BinaryPolynomial secondPolynomial)
    {
        return firstPolynomial.Value ^ secondPolynomial.Value;
    }

    public static BinaryPolynomial operator -(BinaryPolynomial firstPolynomial, BinaryPolynomial secondPolynomial)
    {
        return firstPolynomial.Value ^ secondPolynomial.Value;
    }

    public static BinaryPolynomial operator *(BinaryPolynomial firstPolynomial, BinaryPolynomial secondPolynomial)
    {
        uint result = 0;

        var firstNumberBits = firstPolynomial.ToOpenText();
        var secondNumberBits = secondPolynomial.ToOpenText();

        for (var i = 0; i < firstNumberBits.Length; i++)
        for (var j = 0; j < secondNumberBits.Length; j++)
        {
            var shift = i + j;
            result ^= (firstNumberBits[i] & secondNumberBits[j]) << shift;
        }

        return result;
    }

    public static BinaryPolynomial operator %(BinaryPolynomial polynomial, BinaryPolynomial divisorPolynomial)
    {
        if (divisorPolynomial == Zero)
            throw new DivideByZeroException("Divisor polynomial must be not 0");

        switch ((uint)polynomial, (uint)divisorPolynomial)
        {
            case (0, _):
            case (1, 1):
                return 0;
            case (1, _):
                return 1;
        }

        var (_, result) = Divide(polynomial, divisorPolynomial);
        return result;
    }

    public static BinaryPolynomial operator /(BinaryPolynomial polynomial, BinaryPolynomial divisorPolynomial)
    {
        if (divisorPolynomial == Zero)
            throw new DivideByZeroException("Divisor polynomial must be not 0");

        if (polynomial.Value is 0 or 1)
            return Zero;

        if (divisorPolynomial == One)
            return polynomial;

        var (result, _) = Divide(polynomial, divisorPolynomial);
        return result;
    }


    private static (BinaryPolynomial quotient, BinaryPolynomial remainder) Divide(BinaryPolynomial polynomial,
        BinaryPolynomial divisorPolynomial)
    {
        var result = Zero.ToOpenText();

        var number = polynomial.ToOpenText();
        var divisor = divisorPolynomial.ToOpenText();

        while (number.Length >= divisor.Length)
        {
            var shift = (int)(number.Length - divisor.Length);
            number ^= divisor << shift;
            result ^= One.ToOpenText() << shift;
        }

        return (new BinaryPolynomial(result.Value), new BinaryPolynomial(number.Value));
    }

    #endregion

    #region Converting from/to other formats

    public OpenText ToOpenText()
    {
        return new OpenText(Value);
    }

    public static implicit operator BinaryPolynomial(uint polynomialValue)
    {
        return new BinaryPolynomial(polynomialValue);
    }

    public static implicit operator uint(BinaryPolynomial polynomial)
    {
        return polynomial.Value;
    }

    public static implicit operator BinaryPolynomial(int polynomialValue)
    {
        return new BinaryPolynomial((uint)polynomialValue);
    }

    public static implicit operator BinaryPolynomial(byte polynomialValue)
    {
        return new BinaryPolynomial(polynomialValue);
    }

    public static implicit operator byte(BinaryPolynomial polynomialValue)
    {
        return (byte)polynomialValue.Value;
    }

    public static string ToPotentialForm(uint value, int countBitsInNumber)
    {
        var numberToConvert = new OpenText(value);
        return Enumerable
            .Range(0, countBitsInNumber)
            .Reverse()
            .Where(degree => numberToConvert[degree] == 1)
            .Select(degree => degree switch
            {
                0 => "1",
                1 => "x",
                _ => $"x^{degree}"
            })
            .Aggregate((prev, next) => $"{prev} + {next}");
    }

    #endregion
}