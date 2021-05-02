using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cryptography.Algorithms.Utils;

namespace Cryptography.Algorithms.Symmetric.CipherStrategy
{
    [SymmetricCipherStrategy(SymmetricCipherMode.CipherBlockChaining)]
    public class CipherBlockChainingStrategy : ICipherStrategy
    {
        public byte[] InitializationVector { get; set; }
        public List<byte[]> Encrypt(ISymmetricCipher symmetricCipher, List<byte[]> openText)
        {
            var c = InitializationVector;
            var encryptedMessage = new List<byte[]>();
            
            foreach (var messageBlock in openText)
            {
                var xoredBlock = CipherUtils.XorByteArrays(messageBlock, c);
                var encryptedBlock = symmetricCipher.Encrypt(xoredBlock);
                c = encryptedBlock;
                encryptedMessage.Add(encryptedBlock);
            }

            return encryptedMessage;
        }

        public List<byte[]> Decrypt(ISymmetricCipher symmetricCipher, List<byte[]> cipherText)
        {
            var c = InitializationVector;
            var decryptedMessage = new byte[cipherText.Count][];

            Parallel.For(0, cipherText.Count, 
                blockNumber => decryptedMessage[blockNumber] = symmetricCipher.Decrypt(cipherText[blockNumber]));

            for (int i = 0; i < decryptedMessage.Length; i++)
            {
                decryptedMessage[i] = CipherUtils.XorByteArrays(c, decryptedMessage[i]);
                c = cipherText[i];
            }

            return decryptedMessage.ToList();
        }
    }
}