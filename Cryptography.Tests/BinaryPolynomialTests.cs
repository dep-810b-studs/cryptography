using Cryptography.Arithmetic;
using Xunit;

namespace Cryptography.Tests
{
    public class BinaryPolynomialTests
    {
        [Theory]
        [InlineData(207u, 309u ,55779,"x^15 + x^14 + x^12 + x^11 + x^8 + x^7 + x^6 + x^5 + x + 1")]
        [InlineData(20732u, 30914u ,429298788,"x^28 + x^27 + x^24 + x^23 + x^20 + x^18 + x^17 + x^15 + x^12 + x^10 + x^6 + x^5 + x^2")]
        [InlineData(2072u, 3091u, 6306216, "x^22 + x^21 + x^13 + x^12 + x^11 + x^8 + x^7 + x^5 + x^3")]
        public void MultiplicationShouldWorkCorrect(BinaryPolynomial firstNumber, BinaryPolynomial secondNumber,
            uint  expectedResult,string expectedPolynomial)
        {
            //act
            var actualResult = firstNumber * secondNumber;
            //assert
            Assert.Equal(expectedResult, actualResult.Value);
            Assert.Equal(expectedPolynomial, actualResult.ToString());
        }
    }
}