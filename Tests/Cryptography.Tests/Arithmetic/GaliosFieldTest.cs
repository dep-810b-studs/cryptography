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

        [Theory]
        [InlineData(205,217,283,29)]
        [InlineData(137,15,283,182)]
        [InlineData(123, 34, 283, 15)]
        [InlineData(137,15,287,170)]
        public void MultiplicationInGaliosFieldShouldWorkCorrect(byte firstOperand, byte secondOperand, uint irreduciblePolynomial, byte expectedResult)
        {
            //arrange
            var gfUnderTest = new GaloisField(irreduciblePolynomial);
            //act
            var actualResult = gfUnderTest.Multiply(firstOperand, secondOperand);
            //assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(35, MultiplicativeInverseCalculationWay.Exponentiation,241)]
        [InlineData(35, MultiplicativeInverseCalculationWay.ExtendedEuclideanAlgorithm,241)]
        [InlineData(203, MultiplicativeInverseCalculationWay.Exponentiation,4)]
        [InlineData(203, MultiplicativeInverseCalculationWay.ExtendedEuclideanAlgorithm,4)]
        public void MultiplicativeInverseShouldBeCalculationCorrect(byte number, MultiplicativeInverseCalculationWay way,
            byte expectedInverse)
        {
            //arrange
            var gfUnderTest = new GaloisField();
            
            //act
            var actualInverse = gfUnderTest.MultiplicativeInverse(number, way);
            
            //assert
            Assert.Equal(expectedInverse, actualInverse);
        }

        [Theory]
        [InlineData(120, 0b100011101)]
        [InlineData(255, 0b100011101)]
        [InlineData(1, 0b100011101)]
        [InlineData(0, 0b100011101)]
        public void MappingToAnotherFieldShouldWorkCorrect(byte number, uint irreduciblePolynomial)
        {
            //arrange
            var gfUnderTest = new GaloisField();
            //act
            var inAnotherField = gfUnderTest.ToAnotherField(number, irreduciblePolynomial);
            gfUnderTest.IrreduciblePolynomial = irreduciblePolynomial;
            var inOriginalField = gfUnderTest.ToAnotherField(inAnotherField, 0x11b);
            //assert
            Assert.Equal(number, inOriginalField);
        }
    }
}