using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cryptography.Arithmetic.WorkingWithBits;

namespace Cryptography.Arithmetic.GaloisField
{
    public class GaloisField
    {
        public GaloisField(uint m = 0x11b)
        {
            IrreduciblePolynomial = m;
        }

        public BinaryPolynomial IrreduciblePolynomial { get; set; }

        public byte Multiply(byte firstNumber, byte secondNumber)
        {
            BinaryPolynomial firstNumberPolynomial = firstNumber;
            BinaryPolynomial secondNumberPolynomial = secondNumber;

            var multiplicationResult = firstNumberPolynomial * secondNumberPolynomial;
            var multiplicationByModule = multiplicationResult % IrreduciblePolynomial;

            return multiplicationByModule;
        }
        
        public byte Divide(byte firstNumber, byte secondNumber)
        {
            BinaryPolynomial firstNumberPolynomial = firstNumber;
            BinaryPolynomial secondNumberPolynomial = secondNumber;
            BinaryPolynomial secondNumberPolynomialInverse = MultiplicativeInverseUsingExponentiation(secondNumberPolynomial)

            var multiplicationResult = firstNumberPolynomial * secondNumberPolynomialInverse;
            var multiplicationByModule = multiplicationResult % IrreduciblePolynomial;

            return multiplicationByModule;
        }

        
        public byte Pow(byte number, byte exponent)
        {
            byte result = 1;

            while (exponent != 0)
            {
                if ((exponent & 1) == 1)
                    result = Multiply(result, number);
                result = Multiply(result, result);
                exponent >>= 1;
            }
            
            return result;
        }
        
        public byte MultiplicativeInverse(byte number, MultiplicativeInverseCalculationWay way)
        {
            if (number == 0)
                throw new ArgumentOutOfRangeException();

            return way switch
            {
                MultiplicativeInverseCalculationWay.ExtendedEuclideanAlgorithm => MultiplicativeInverseUsingExtendedGCD(
                    number),
                MultiplicativeInverseCalculationWay.Exponentiation => MultiplicativeInverseUsingExponentiation(number),
                _ => throw new ArgumentOutOfRangeException()
            };
        }


        public (BinaryPolynomial d, BinaryPolynomial x, BinaryPolynomial y) ExtendedEuclideanAlgorithm(
            BinaryPolynomial a, BinaryPolynomial b)
        {
            if (b == BinaryPolynomial.Zero)
            {
                return (a, 1, 0);
            }

            var x1 = BinaryPolynomial.One;
            var y1 = BinaryPolynomial.Zero;
            var x2 = BinaryPolynomial.Zero;
            var y2 = BinaryPolynomial.One;
            
            while (b > 0)
            {
                var q = Divide(a,b);
            
                BinaryPolynomial t;
                
                t = b.Copy();
                b = a % b;
                a = t.Copy();
                
                t = x2.Copy();
                x2 = x1 - Multiply(q, x2);
                x1 = t.Copy();
                
                t = y2.Copy();
                y2 = y1 - Multiply(q, y2);
                y1 = t.Copy();
            }
            
            return (a, x1, y1);
        }

        public byte ToAnotherField(byte number, uint fieldIrreduciblePolynomial)
        {
            var mapper = (IrreduciblePolynomial.Value ^ fieldIrreduciblePolynomial) % 256;
            uint mappedValue = number ^ mapper;
            return (byte)mappedValue;
        }

        private byte MultiplicativeInverseUsingExtendedGCD(byte number)
        {
            var (_, inverse, _) = BinaryPolynomial.ExtendedEuclideanAlgorithm(number, IrreduciblePolynomial);
            return inverse;
        }
        /// <summary>
        /// Using equation a^-1=a^254
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private byte MultiplicativeInverseUsingExponentiation(byte number) => Pow(number, 254);

        /// <summary>
        /// GF(256) = x^7 + x^6 + x^5 + x^4 + x^3 + x^2 + x + 1
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>

        public static string ToPotentialForm(byte number) => BinaryPolynomial.ToPotentialForm(number, 8);
    }
}     