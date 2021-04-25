using System;
using System.Collections.Generic;
using System.Linq;
using Cryptography.Arithmetic.WorkingWithBits;

namespace Cryptography.Arithmetic
{
    public class BinaryPolynomial
    {
        public uint Value { get; }

        public BinaryPolynomial(uint value)
        {
            Value = value;
        }
        
        public override string ToString() => ToPotentialForm(Value, 32);

        public BinaryPolynomial Copy() => new (Value);
        
        public static (BinaryPolynomial d, BinaryPolynomial x, BinaryPolynomial y) ExtendedEuclideanAlgorithm(
            BinaryPolynomial a, BinaryPolynomial b)
        {
            if (b == Zero)
            {
                return (a, 1, 0);
            }

            BinaryPolynomial x1 = 1, y1 = 0, x2 = 0, y2 =1;
            
            while (b > 0)
            {
                var q = a % b;

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
            
            return (a, y1, x1);
        }
        
        #region Arithmetic Operations
        public static BinaryPolynomial operator +(BinaryPolynomial firstPolynomial, BinaryPolynomial secondPolynomial) =>
            firstPolynomial.Value ^ secondPolynomial.Value;
        public static BinaryPolynomial operator -(BinaryPolynomial firstPolynomial, BinaryPolynomial secondPolynomial) =>
            firstPolynomial.Value ^ secondPolynomial.Value;

        public static BinaryPolynomial operator *(BinaryPolynomial firstPolynomial, BinaryPolynomial secondPolynomial)
        {
            uint result = 0;

            var firstNumberBits = firstPolynomial.ToOpenText();
            var secondNumberBits = secondPolynomial.ToOpenText();
            
            for (var i = 0; i < firstNumberBits.Length; i++)
            {
                for (var j = 0; j < secondNumberBits.Length; j++)
                {
                    var shift = i + j;
                    result ^= (uint)((firstNumberBits[i] & secondNumberBits[j]) << shift);
                }
            }

            return result;
        }
        
        public static BinaryPolynomial operator %(BinaryPolynomial polynomial, BinaryPolynomial divisorPolynomial)
        {
            if (divisorPolynomial == 0)
                throw new DivideByZeroException("Divisor polynomial must be not 0");
            
            switch ((uint)polynomial, (uint)divisorPolynomial)
            {
                case (0,_):
                case (1,1):
                    return 0;
                case (1,_):
                    return 1;
            }

            var number = polynomial.ToOpenText();
            var divisor = divisorPolynomial.ToOpenText();
            
            var numberMaxPower = number.GetDegreeOfTwoThatNeighborsOfNumber();
            var divisorMaxPower = divisor.GetDegreeOfTwoThatNeighborsOfNumber();
            
            while (numberMaxPower >= divisorMaxPower)
            {
                var shift = (numberMaxPower - divisorMaxPower);
                number ^= divisor << shift;
                numberMaxPower = number.GetDegreeOfTwoThatNeighborsOfNumber();
            }
            
            return new BinaryPolynomial(number.Value);
        }

        #endregion
        
        #region Converting from/to other formats
        public OpenText ToOpenText() => new (Value);
        public static implicit operator BinaryPolynomial(uint polynomialValue) => new(polynomialValue);
        public static implicit operator uint(BinaryPolynomial polynomial) => polynomial.Value;
        public static implicit operator BinaryPolynomial(int polynomialValue) => new((uint)polynomialValue);
        public static implicit operator BinaryPolynomial(byte polynomialValue) => new((uint)polynomialValue);
        public static implicit operator byte(BinaryPolynomial polynomialValue) => (byte)polynomialValue.Value;

        public static string ToPotentialForm(uint value, int countBitsInNumber)
        {
            var numberToConvert = new OpenText(value);
            return Enumerable
                .Range(0,countBitsInNumber)
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

        public override bool Equals(object? obj)
        {
            if (obj is BinaryPolynomial binaryPolynomial)
                return Value == binaryPolynomial.Value;
            
            return base.Equals(obj);
        }

        public static BinaryPolynomial Zero => new(0);
    }
}