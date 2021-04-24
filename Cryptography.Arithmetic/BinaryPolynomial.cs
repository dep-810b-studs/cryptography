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

        #region Arithmetic Operations
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
    }
}