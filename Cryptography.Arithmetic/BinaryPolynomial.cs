using System;
using System.Collections.Generic;
using System.Linq;
using Cryptography.Arithmetic.WorkingWithBits;
using Microsoft.VisualBasic;

namespace Cryptography.Arithmetic
{
    public class BinaryPolynomial
    {
        public uint Value { get; }

        public BinaryPolynomial(uint value)
        {
            Value = value;
        }

        public static BinaryPolynomial operator *(BinaryPolynomial firstPolynomial, BinaryPolynomial secondPolynomial)
        {
            int result = 0;

            var a = (int)firstPolynomial.Value;
            var b = (int)secondPolynomial.Value;

            while (a != 0)
            {
                result ^= (a & 1) * b;
                b <<= 1;
                a >>= 1;
            }

            
            return new BinaryPolynomial((uint)result);
        }

        public OpenText ToOpenText() => new (Value);
        
        public override string ToString() => ToPotentialForm(Value, 32);
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

        public static implicit operator BinaryPolynomial(uint polynomialValue) => new(polynomialValue);

    }
}