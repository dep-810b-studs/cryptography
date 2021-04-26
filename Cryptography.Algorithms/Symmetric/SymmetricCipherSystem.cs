using Cryptography.Algorithms.Utils;

namespace Cryptography.Algorithms.Symmetric
{
    interface ISymmetricSystem
    {
        string Encrypt(string openMessage, string key);
        string Decrypt(string encryptedMessage, string key);
    }
    
    public class SymmetricSystem : ISymmetricSystem
    {
        private readonly ISymmetricCipher _symmetricCipher;
        private readonly IMessageConvertor _messageConvertor;

        public SymmetricSystem(ISymmetricCipher rijandelCipher, IMessageConvertor messageConvertor)
        {
            _symmetricCipher = rijandelCipher;
            _messageConvertor = messageConvertor;
        }
        
        public SymmetricCipherMode SymmetricCipherMode { get; set; }

        public string Encrypt(string openMessage, string key) =>
            EncryptionConversion(openMessage, key, CipherAction.Encrypt);

        public string Decrypt(string encryptedMessage, string key) =>
            EncryptionConversion(encryptedMessage, key, CipherAction.Decrypt);

        private string EncryptionConversion(string message, string key, CipherAction cipherAction)
        {
            var messageInBytes = _messageConvertor.ConvertToBytes(message, 0);
            var keyInBytes = _messageConvertor.ConvertToBytes(message, 0);

            var processedBytes = cipherAction switch
            {
                CipherAction.Encrypt => _symmetricCipher.Encrypt(messageInBytes, keyInBytes),
                CipherAction.Decrypt => _symmetricCipher.Decrypt(messageInBytes, keyInBytes)
            };

            return _messageConvertor.ConvertToString(processedBytes);
        }
    }
}