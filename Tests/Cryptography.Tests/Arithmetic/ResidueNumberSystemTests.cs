using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cryptography.Arithmetic.ResidueNumberSystem;
using Xunit;

namespace Cryptography.Tests;

public class ResidueNumberSystemTests
{
    private readonly ResidueNumberSystem _residueNumberSystemUnderTest;

    public ResidueNumberSystemTests()
    {
        _residueNumberSystemUnderTest = new ResidueNumberSystem();
    }

    [Theory]
    [InlineData(2, 37, 77, 51)]
    [InlineData(11, 3, 3, 2)]
    public void ModPowShouldWorkCorrect(ulong num, ulong pow, ulong module, ulong expectedResult)
    {
        //arrange
        _residueNumberSystemUnderTest.Module = module;
        //act
        var actualResult = _residueNumberSystemUnderTest.Pow(num, pow);
        //assert
        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [ClassData(typeof(PrimesNumbersTestData))]
    public void ReshetoEratosfenaShouldWorkCorrect(int count, IEnumerable<int> expectedResult)
    {
        //act
        var numbers = _residueNumberSystemUnderTest.GetSimpleNumbersLessThenM(count).ToList();
        //assert
        Assert.Equal(expectedResult, numbers);
    }


    private class PrimesNumbersTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                30,
                new List<int> { 1, 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 }
            };
            yield return new object[]
            {
                10,
                new List<int> { 1, 2, 3, 5, 7 }
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}