using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cryptography.Algorithms.Utils;

namespace Cryptography.Algorithms.Symmetric.CipherStrategy;

[SymmetricCipherStrategy(SymmetricCipherMode.OutputFeedback)]
public class OutputFeedbackStrategy : ICipherStrategy
{
    public byte[] InitializationVector { get; set; }

    public List<byte[]> Encrypt(ISymmetricCipher symmetricCipher, List<byte[]> openText)
    {
        return EncryptionConvertion(symmetricCipher, openText);
    }

    public List<byte[]> Decrypt(ISymmetricCipher symmetricCipher, List<byte[]> cipherText)
    {
        return EncryptionConvertion(symmetricCipher, cipherText);
    }

    private List<byte[]> EncryptionConvertion(ISymmetricCipher symmetricCipher, List<byte[]> cipherText)
    {
        var c = InitializationVector;

        var encryptedMessage = new byte[cipherText.Count][];

        for (var i = 0; i < encryptedMessage.Length; i++)
        {
            encryptedMessage[i] = symmetricCipher.Encrypt(c);
            c = encryptedMessage[i];
        }

        Parallel.For(0, encryptedMessage.Length,
            blockNumber => encryptedMessage[blockNumber] =
                CipherUtils.XorByteArrays(cipherText[blockNumber], encryptedMessage[blockNumber]));

        return encryptedMessage.ToList();
    }
}