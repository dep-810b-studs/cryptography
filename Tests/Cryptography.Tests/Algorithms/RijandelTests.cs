using System;
using System.Collections;
using System.Collections.Generic;
using Cryptography.Algorithms.Rijandel;
using Cryptography.Algorithms.Symmetric;
using Xunit;

namespace Cryptography.Tests
{
    public class RijandelTests
    {
        [Theory]
        [InlineData(16, CipherBlockSize.Small)]
        [InlineData(24, CipherBlockSize.Middle)]
        [InlineData(32, CipherBlockSize.Big)]
        public void ShouldResultAfterEncryptionThenDecryptionBeSameAsOriginaldata(int blockSize, CipherBlockSize cipherBlockSize)
        {
            //arrange
            var openText = new byte[blockSize];
            var key = new byte[blockSize];

            var random = new Random();
            
            random.NextBytes(openText);
            random.NextBytes(key);

            var rijandelUnderTest = new RijandelCipher();
            rijandelUnderTest.CipherBlockSize = cipherBlockSize; 
            
            rijandelUnderTest.CreateRoundKeys(key);
            
            //act
            var cipherText = rijandelUnderTest.Encrypt(openText);
            var actualdecryptionResult = rijandelUnderTest.Decrypt(cipherText);
            //assert
            Assert.Equal(openText, actualdecryptionResult);
        }
    }
}