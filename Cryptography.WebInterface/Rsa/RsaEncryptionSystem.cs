using System;
using System.Linq;
using Cryptography.Algorithms.RSA;
using Cryptography.Arithmetic.ResidueNumberSystem;

namespace Cryptography.WebInterface.Rsa
{
    public interface IRsaEncryptionSystem
    {
        string Encrypt(string message, string P, string Q, string e);
        string Decrypt(string message, string N, string D);
        ulong GetRandomPrimeNumber(int max, Func<int, bool> isNumberValid);
    }

    internal class RsaEncryptionSystem : IRsaEncryptionSystem
    {
        private readonly IMessageConvertor _messageConvertor;
        private readonly IRSACipher _rsaCipher;
        private readonly IResidueNumberSystem _residueNumberSystem;
        private readonly Random _random = new ();

        public RsaEncryptionSystem(IMessageConvertor messageConvertor, IRSACipher rsaCipher, IResidueNumberSystem residueNumberSystem)
        {
            _messageConvertor = messageConvertor;
            _rsaCipher = rsaCipher;
            _residueNumberSystem = residueNumberSystem;
        }

        public string Encrypt(string message, string P, string Q, string e)
        {
            var convertedMessage = _messageConvertor.ConvertToLong(message);
            var convertedP = UInt32.Parse(P);
            var convertedQ = UInt32.Parse(Q);
            var convertedE = UInt64.Parse(e);
            var encryptedMessage = _rsaCipher.EnCrypt(convertedMessage,
                convertedP, convertedQ, convertedE);

            return encryptedMessage.CipherText.ToString();
        }

        public string Decrypt(string message, string N, string D)
        {
            var convertedMessage = UInt64.Parse(message); 
            var convertedN = UInt32.Parse(N);
            var convertedD = UInt64.Parse(D);

            var encryptionResult = new RSAEncryptionResult
            {
                CipherText = convertedMessage,
                PublicKey = (convertedN,0),
                SecretKey = (convertedD, 0, 0)
            };

            var decryptedMessage = _rsaCipher.DeCrypt(encryptionResult);

            return _messageConvertor.ConvertToString(decryptedMessage);
        }

        public ulong GetRandomPrimeNumber(int max, Func<int, bool> isNumberValid)
        {
            var primeNumbers = _residueNumberSystem.GetSimpleNumbersLessThenM(max).ToArray();

            while (true)
            {
                var randomPrime = primeNumbers[_random.Next(primeNumbers.Length - 1)];
                if (isNumberValid(randomPrime))
                    return (ulong) randomPrime;
            }
        }
    }
    
}