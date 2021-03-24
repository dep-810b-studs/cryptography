using System;
using Cryptography.Arithmetic.ResidueNumberSystem;

namespace Cryptography.Algorithms.RSA
{
    public class RSA 
    {
        private readonly ResidueNumberSystem _residueNumberSystem;
        private readonly RSASettings _rsaSettings;
        
        public RSA(ResidueNumberSystem residueNumberSystem, RSASettings rsaSettings)
        {
            _residueNumberSystem = residueNumberSystem;
            _rsaSettings = rsaSettings;
        }
        
        public RSAEncryptionResult EnCrypt(ulong message, uint p, uint q, ulong encryptionExponent)
        {
            AssertPrimeNumberBitsCountCorrect(p);
            AssertPrimeNumberBitsCountCorrect(q);
            AssertEncryptionExponentCorrect(encryptionExponent, p, q);
            
            _residueNumberSystem.Module = p * q;

            ulong eilerFunctionValue = _residueNumberSystem.CalculateEylerFunction(p, q);
            var (decryptionExponent, _, _) = 
                _residueNumberSystem.ExtendedEuclideanAlgorithm(encryptionExponent, eilerFunctionValue);

            var cipherText = _residueNumberSystem.Pow(message, eilerFunctionValue);

            var encryptionResult = new RSAEncryptionResult()
            {
                CipherText = cipherText,
                PublicKey = (_residueNumberSystem.Module, encryptionExponent),
                SecretKey = (decryptionExponent, p, q)
            };
            
            return encryptionResult;
        }

        public ulong DeCrypt(RSAEncryptionResult rsaEncryptionResult)
        {
            AssertPrimeNumberBitsCountCorrect(rsaEncryptionResult.SecretKey.p);
            AssertPrimeNumberBitsCountCorrect(rsaEncryptionResult.SecretKey.p);
            AssertEncryptionExponentCorrect(rsaEncryptionResult.PublicKey.E, rsaEncryptionResult.SecretKey.p, rsaEncryptionResult.SecretKey.q);

            _residueNumberSystem.Module = rsaEncryptionResult.PublicKey.N;

            var message = _residueNumberSystem.Pow(rsaEncryptionResult.CipherText, rsaEncryptionResult.SecretKey.d);

            return message;
        }

        #region Utils

        private uint GetNumberLenInBits(uint number)
        {
            uint countBits = 0;
            
            while (number > 0)
            {
                number >>= 1;
                countBits++;
            }

            return countBits;
        }

        private void AssertPrimeNumberBitsCountCorrect(uint number)
        {
            var numberLenInBits = GetNumberLenInBits(number);

            if (numberLenInBits < _rsaSettings.PrimeNumberCountBitsMin ||
                numberLenInBits > _rsaSettings.PrimeNumberCountBitsMax)
                throw new ArgumentOutOfRangeException(nameof(number),
                    $"Prime number bits len should be in range: [{_rsaSettings.PrimeNumberCountBitsMin},{_rsaSettings.PrimeNumberCountBitsMax}].");
        }

        private void AssertEncryptionExponentCorrect(ulong exponent, uint p, uint q)
        {
            var encryptedExponentCorrect =
                _residueNumberSystem.GreatestCommonDivisor(exponent, p - 1) == 1 ||
                _residueNumberSystem.GreatestCommonDivisor(exponent, q - 1) == 1;

            if (!encryptedExponentCorrect)
                throw new ArgumentException("The encryption exponent is not mutually prime with p-1 or q-1.");
        }

        #endregion
    }
}
