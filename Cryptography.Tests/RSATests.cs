using System.Text;
using Cryptography.Algorithms;
using Xunit;

namespace Cryptography.Tests
{
    public class RSATests
    {
        private readonly ICipher _cipherUnderTest;

        public RSATests()
        {
            _cipherUnderTest = new RSA();
        }

        [Fact]
        public void RsaShouldEncryptCorrectly()
        {
            //arrange
            var openText = "hello, world";
            //act
            var res = _cipherUnderTest.EnCrypt(Encoding.ASCII.GetBytes(openText));
            var resInTextFormat = Encoding.ASCII.GetString(res);
            //assert
            Assert.Equal(resInTextFormat, openText);
        }
    }
}