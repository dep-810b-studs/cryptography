using Cryptography.Arithmetic.GaloisField;
using Xunit;

namespace Cryptography.Tests
{
    public class GaloisFieldTest
    {
        [Theory]
        [InlineData(123, "x^6 + x^5 + x^4 + x^3 + x + 1")]
        [InlineData(243, "x^7 + x^6 + x^5 + x^4 + x + 1")]
        public void ShouldCorrectRepresentInPotentialForm(byte number, string expectedPolynomial)
        {
           //act
           var actualPolynomial = GaloisField.ToPotentialForm(number);
           //assert
           Assert.Equal(expectedPolynomial, actualPolynomial);
        }
    }
}