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
        [InlineData(123,11,13,113,17,41)]
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
        
        [Theory]
        [InlineData(7500641,9973,9967,37)]
        [InlineData(2,7,11,37)]
        [InlineData(123,11,13,113)]
        [InlineData(123,9973,9967,65537)]
        public void RsaShouldEncryptAndThenDecryptCorrectly(ulong message, uint p, uint q, ulong E)
        {
            //act
            var actualCipherResult = _rsaCipher.EnCrypt(message, p, q, E);
            var actualDecryptionResult = _rsaCipher.DeCrypt(actualCipherResult);
            //assert
            Assert.Equal(message, actualDecryptionResult);
        }
    }
}