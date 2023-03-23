using System;
using Cryptography.Algorithms.Symmetric.CipherManager;

namespace Cryptography.Algorithms.Symmetric.CipherSystem;

public interface ISymmetricSystem
{
    byte[] HandleEncryption(CipherAction cipherAction, SymmetricCipherMode cipherMode,
        CipherBlockSize cipherBlockSize,
        byte[] data, byte[] key, byte[] initializationVector = null);

    byte[] GenerateRandomKey(CipherBlockSize cipherBlockSize = CipherBlockSize.Small);
}

public class SymmetricSystem : ISymmetricSystem
{
    private readonly ISymmetricCipherManager _symmetricCipherManager;

    public SymmetricSystem(ISymmetricCipherManager symmetricCipherManager)
    {
        _symmetricCipherManager = symmetricCipherManager;
    }

    #region System API
    public byte[] HandleEncryption(CipherAction cipherAction, 
        SymmetricCipherMode cipherMode,
        CipherBlockSize cipherBlockSize,
        byte[] data, byte[] key, byte[] initializationVector = null)
    {
        _symmetricCipherManager.Key = key;
        _symmetricCipherManager.CipherMode = cipherMode;
        _symmetricCipherManager.CipherBlockSize = cipherBlockSize;

        if (cipherMode is not SymmetricCipherMode.ElectronicCodeBook)
        {
            _symmetricCipherManager.InitializationVector = initializationVector;
        }

        var processedData = cipherAction switch
        {
            CipherAction.Encrypt => _symmetricCipherManager.Encrypt(data),
            CipherAction.Decrypt => _symmetricCipherManager.Decrypt(data)
        };

        return processedData;
    }

    public byte[] GenerateRandomKey(CipherBlockSize cipherBlockSize = CipherBlockSize.Small)
    {
        var random = new Random();
        var randomKey = new byte[(int)cipherBlockSize / 8];
        random.NextBytes(randomKey);
        return randomKey;
    }

    #endregion
}