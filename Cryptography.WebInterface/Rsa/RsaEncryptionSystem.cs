using System;
using Cryptography.Algorithms.RSA;

namespace Cryptography.WebInterface.Rsa
{
    public interface IRsaEncryptionSystem
    {
        string Encrypt(string message, string P, string Q, string e);
        string Decrypt(string message, string N, string D);
    }

    internal class RsaEncryptionSystem : IRsaEncryptionSystem
    {
        private readonly IMessageConvertor _messageConvertor;
        private readonly IRSACipher _rsaCipher;

        public RsaEncryptionSystem(IMessageConvertor messageConvertor, IRSACipher rsaCipher)
        {
            _messageConvertor = messageConvertor;
            _rsaCipher = rsaCipher;
        }

        public string Encrypt(string message, string P, string Q, string e)
        {
            var convertedMessage = _messageConvertor.ConvertToLong(message);
            var convertedP = UInt32.Parse(P);
            var convertedQ = UInt32.Parse(Q);
            var convertedE = UInt64.Parse(e);
            var encryptedMessage = _rsaCipher.EnCrypt(convertedMessage,
                convertedP, convertedQ, convertedE);

            return _messageConvertor.ConvertToString(encryptedMessage.CipherText);
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
    }
    
}