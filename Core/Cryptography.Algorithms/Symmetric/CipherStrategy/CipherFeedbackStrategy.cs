using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cryptography.Algorithms.Utils;

namespace Cryptography.Algorithms.Symmetric.CipherStrategy;

[SymmetricCipherStrategy(SymmetricCipherMode.CipherFeedback)]
public class CipherFeedbackStrategy : ICipherStrategy
{
    public byte[] InitializationVector { get; set; }

    public List<byte[]> Encrypt(ISymmetricCipher symmetricCipher, List<byte[]> openText)
    {
        var c = InitializationVector;
        var encryptedMessage = new List<byte[]>();

        foreach (var messageBlock in openText)
        {
            var encryptedBlock = symmetricCipher.Encrypt(c);
            var xoredBlock = CipherUtils.XorByteArrays(encryptedBlock, messageBlock);
            c = xoredBlock;
            encryptedMessage.Add(xoredBlock);
        }

        return encryptedMessage;
    }

    public List<byte[]> Decrypt(ISymmetricCipher symmetricCipher, List<byte[]> cipherText)
    {
        var decryptedMessage = new byte[cipherText.Count][];

        Parallel.For(0, cipherText.Count,
            blockNumber =>
            {
                var currentBlock = blockNumber > 0 ? cipherText[blockNumber - 1] : InitializationVector;
                var decryptdData = symmetricCipher.Encrypt(currentBlock);
                var openText = CipherUtils.XorByteArrays(decryptdData, cipherText[blockNumber]);
                decryptedMessage[blockNumber] = openText;
            });

        return decryptedMessage.ToList();
    }
}