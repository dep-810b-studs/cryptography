using Cryptography.Algorithms.RSA;
using Cryptography.Arithmetic.ResidueNumberSystem;
using Microsoft.Extensions.Options;
using Xunit;

namespace Cryptography.Tests
{
    public class RSATests
    {
        private readonly RSACipher _rsaCipher;
        
        public RSATests()
        {
            var rsaSettings = new RSASettings()
            {
                PrimeNumberCountBitsMax = 16,
                PrimeNumberCountBitsMin = 0
            };

            var residueNumberSystem = new ResidueNumberSystem();
            
            _rsaCipher = new RSACipher(residueNumberSystem, Options.Create(rsaSettings));
        }

        [Theory]
        [InlineData(2,7,11,37,13,51)]
        public void RsaShouldEncryptCorrectly(ulong message, uint p, uint q, ulong E,ulong d, ulong cipherMessage)
        {
            //act
            var actualCipherResult = _rsaCipher.EnCrypt(message, p, q, E);
            //assert
            Assert.Equal(cipherMessage, actualCipherResult.CipherText);
            Assert.Equal(d,actualCipherResult.SecretKey.d);
        }
        
        [Theory]
        [InlineData(51, 13,7,11,37, 2)]
        public void RsaShouldDecryptCorrectly(ulong cipherMessage, ulong d, ulong p, ulong q, ulong e, ulong expectedMessage)
        {
            //arrange
            var encryptionResult = new RSAEncryptionResult()
            {
                CipherText = cipherMessage,
                SecretKey = (d, 0, 0),
                PublicKey = (p * q, e)
            };
            //act
            var actualDecryptionResult = _rsaCipher.DeCrypt(encryptionResult);
            //assert
            Assert.Equal(expectedMessage, actualDecryptionResult);
        }
    }
}