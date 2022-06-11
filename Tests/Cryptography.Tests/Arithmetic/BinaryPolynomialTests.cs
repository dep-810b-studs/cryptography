using Cryptography.Arithmetic;
using Xunit;

namespace Cryptography.Tests
{
    public class BinaryPolynomialTests
    {
        [Theory]
        [InlineData(207u, 309u ,55779,"x^15 + x^14 + x^12 + x^11 + x^8 + x^7 + x^6 + x^5 + x + 1")]
        //[InlineData(20732u, 30914u ,429298788,"x^28 + x^27 + x^24 + x^23 + x^20 + x^18 + x^17 + x^15 + x^12 + x^10 + x^6 + x^5 + x^2")]
        [InlineData(2072u, 3091u, 6306216, "x^22 + x^21 + x^13 + x^12 + x^11 + x^8 + x^7 + x^5 + x^3")]
        [InlineData(5u,40u,136,"x^7 + x^3")]
        public void MultiplicationShouldWorkCorrect(uint originalFirstNumber, uint originalSecondNumber,
            uint  expectedResult,string expectedPolynomial)
        {
            //arrange
            BinaryPolynomial firstNumber = originalFirstNumber;
            BinaryPolynomial secondNumber = originalSecondNumber;
            //act
            var actualResult = firstNumber * secondNumber;
            //assert
            Assert.Equal(expectedResult, actualResult.Value);
            Assert.Equal(expectedPolynomial, actualResult.ToString());
        }

        [Theory]
        [InlineData(303u,256u,47u)]
        [InlineData(0u,256u,0u)]
        [InlineData(1u,1u,0u)]
        [InlineData(1u,134u,1u)]
        public void DivisionModuleShouldWorkCorrect(uint originalNumber, uint originalDivision, 
            uint expectedResult)
        {
            //arrange
            BinaryPolynomial number = originalNumber;
            BinaryPolynomial division = originalDivision;
            //act
            var actualResult = number % division;
            //assert
            Assert.Equal(expectedResult, actualResult.Value);
        }
        
        [Theory]
        [InlineData(136u,40u,5u)]
        [InlineData(136u,5u,40u)]
        public void DivisionShouldWorkCorrect(uint originalNumber, uint originalDivision, 
            uint expectedResult)
        {
            //arrange
            BinaryPolynomial number = originalNumber;
            BinaryPolynomial division = originalDivision;
            //act
            var actualResult = number / division;
            //assert
            Assert.Equal(expectedResult, actualResult.Value);
        }
    }
}