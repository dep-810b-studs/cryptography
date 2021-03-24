using System.Reflection;
using Cryptography.Arithmetic.ResidueNumberSystem;
using Xunit;

namespace Cryptography.Tests
{
    public class ResidueNumberSystemTests
    {
        private readonly ResidueNumberSystem _residueNumberSystemUnderTest;

        public ResidueNumberSystemTests()
        {
            _residueNumberSystemUnderTest = new ResidueNumberSystem();
        }

        [Theory]
        [InlineData(2,37,77,51)]
        [InlineData(11,3,3,2)]
        public void ModPowShouldWorkCorrect(ulong num, ulong pow, ulong module, ulong expectedResult)
        {
            //arrange
            _residueNumberSystemUnderTest.Module = module;
            //act
            var actualResult = _residueNumberSystemUnderTest.Pow(num, pow);
            //assert
            Assert.Equal(expectedResult, actualResult);
        }
}
}