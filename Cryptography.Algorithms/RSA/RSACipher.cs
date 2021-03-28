﻿using System;
using Cryptography.Arithmetic.ResidueNumberSystem;
using Microsoft.Extensions.Options;

namespace Cryptography.Algorithms.RSA
{
    public interface IRSACipher
    {
        RSAEncryptionResult EnCrypt(ulong message, uint p, uint q, ulong encryptionExponent);
        ulong DeCrypt(RSAEncryptionResult rsaEncryptionResult);
    }
    
    public class RSACipher : IRSACipher
    {
        private readonly IResidueNumberSystem _residueNumberSystem;
        private readonly RSASettings _rsaSettings;
        
        public RSACipher(IResidueNumberSystem residueNumberSystem, IOptions<RSASettings> rsaSettings)
        {
            _residueNumberSystem = residueNumberSystem;
            _rsaSettings = rsaSettings.Value;
        }
        
        public RSAEncryptionResult EnCrypt(ulong message, uint p, uint q, ulong encryptionExponent)
        {
            AssertPrimeNumberBitsCountCorrect(p);
            AssertPrimeNumberBitsCountCorrect(q);
            AssertEncryptionExponentCorrect(encryptionExponent, p, q);
            
            _residueNumberSystem.Module = p * q;

            AssertMessageSizeCorrect(message, _residueNumberSystem.Module);

            ulong eylerFunctionValue = _residueNumberSystem.CalculateEylerFunction(p, q);
            //  var ( _, a,b) = 
            //       _residueNumberSystem.ExtendedEuclideanAlgorithm(encryptionExponent, eylerFunctionValue);
            //
            //  var d = Math.Min(a, b);
            //  
            // var decryptionExponent =  d % _residueNumberSystem.Module;

            var decryptionExponent = _residueNumberSystem.MultiplicativeInverse(encryptionExponent, eylerFunctionValue);

            Console.WriteLine($"Encrypting.Message = {message}");
            Console.WriteLine($"Encrypting.p = {p}");
            Console.WriteLine($"Encrypting.q = {q}");
            Console.WriteLine($"Encrypting.e = {encryptionExponent}");
            Console.WriteLine($"Encrypting.d = {decryptionExponent}");
            Console.WriteLine($"Encrypting.N = {_residueNumberSystem.Module}");
            
            var cipherText = _residueNumberSystem.Pow(message, encryptionExponent);

            Console.WriteLine($"Encrypting.Cipher Text = {cipherText}");

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
            //AssertPrimeNumberBitsCountCorrect(rsaEncryptionResult.SecretKey.p);
            //AssertPrimeNumberBitsCountCorrect(rsaEncryptionResult.SecretKey.p);
            //AssertEncryptionExponentCorrect(rsaEncryptionResult.PublicKey.E, rsaEncryptionResult.SecretKey.p, rsaEncryptionResult.SecretKey.q);

            _residueNumberSystem.Module = rsaEncryptionResult.PublicKey.N;
            
            Console.WriteLine($"Decrypting.Cipher Text = {rsaEncryptionResult.CipherText}");

            var message = _residueNumberSystem.Pow(rsaEncryptionResult.CipherText, rsaEncryptionResult.SecretKey.d);

            Console.WriteLine($"Decrypting.Message = {message}");

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

        private void AssertMessageSizeCorrect(ulong message, ulong module)
        {
            if (message >= module)
                throw new ArgumentException("Message should be less then N");
        }


        #endregion
    }
}
