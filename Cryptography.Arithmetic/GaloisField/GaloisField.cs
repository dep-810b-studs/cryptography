using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cryptography.Arithmetic.WorkingWithBits;

namespace Cryptography.Arithmetic.GaloisField
{
    public class GaloisField
    {
        public GaloisField(uint m)
        {
            IrreduciblePolynomial = m;
        }

        public BinaryPolynomial IrreduciblePolynomial { get; init; }

        public uint Multiply(byte firstNumber, byte secondNumber)
        {
            BinaryPolynomial firstNumberPolynomial = firstNumber;
            BinaryPolynomial secondNumberPolynomial = secondNumber;

            var multiplicationResult = firstNumberPolynomial * secondNumberPolynomial;
            var multiplicationByModule = multiplicationResult % IrreduciblePolynomial;

            return multiplicationByModule;
        }
        
        /// <summary>
        /// GF(256) = x^7 + x^6 + x^5 + x^4 + x^3 + x^2 + x + 1
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>

        public static string ToPotentialForm(byte number) => BinaryPolynomial.ToPotentialForm(number, 8);
    }
}     