using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptography.Algorithms.Symmetric.CipherStrategy;

[SymmetricCipherStrategy(SymmetricCipherMode.ElectronicCodeBook)]
public class ElectronicCodeBookStrategy : ICipherStrategy
{
    /// <summary>
    ///     This field not used and placed here only for supporting common API
    /// </summary>
    public byte[] InitializationVector { get; set; }

    public List<byte[]> Encrypt(ISymmetricCipher symmetricCipher, List<byte[]> openText)
    {
        return EncryptionConvertion(symmetricCipher, openText, CipherAction.Encrypt);
    }

    public List<byte[]> Decrypt(ISymmetricCipher symmetricCipher, List<byte[]> cipherText)
    {
        return EncryptionConvertion(symmetricCipher, cipherText, CipherAction.Decrypt);
    }

    private List<byte[]> EncryptionConvertion(ISymmetricCipher symmetricCipher, List<byte[]> data,
        CipherAction cipherAction)
    {
        var encryptedMessageblocks = new byte[data.Count][];

        Func<byte[], byte[]> cipherConvertion = cipherAction switch
        {
            CipherAction.Encrypt => symmetricCipher.Encrypt,
            CipherAction.Decrypt => symmetricCipher.Decrypt
        };

         Parallel.For(0, data.Count,
            blockNumber => { encryptedMessageblocks[blockNumber] = cipherConvertion(data[blockNumber]); });

        return encryptedMessageblocks.ToList();
    }
}