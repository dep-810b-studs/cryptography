using System;
using System.IO;
using System.Threading.Tasks;
using Cryptography.Algorithms.Symmetric.CipherManager;

namespace Cryptography.Algorithms.Symmetric.CipherSystem;

public interface ISymmetricSystem
{
    void HandleEncryption(EncryptionParams encryptionParams);

    Task<byte[]> HandleEncryption(CipherAction cipherAction, SymmetricCipherMode cipherMode,
        CipherBlockSize cipherBlockSize,
        Stream data, Stream key, Stream initilizationVector);

    void GenerateAndSaveRandomKeyToFile(string fileName, CipherBlockSize keySize);
}

public class SymmetricSystem : ISymmetricSystem
{
    private readonly ISymmetricCipherManager _symmetricCipherManager;

    public SymmetricSystem(ISymmetricCipherManager symmetricCipherManager)
    {
        _symmetricCipherManager = symmetricCipherManager;
    }

    #region System API

    public void HandleEncryption(EncryptionParams encryptionParams)
    {
        var dataToProcess = ReadAllBytesFromFile(encryptionParams.InputFileName);
        var key = ReadAllBytesFromFile(encryptionParams.KeyFileName);

        _symmetricCipherManager.Key = key;
        _symmetricCipherManager.CipherMode = encryptionParams.SymmetricCipherMode;
        _symmetricCipherManager.CipherBlockSize = encryptionParams.CipherBlockSize;

        if (encryptionParams.SymmetricCipherMode is not SymmetricCipherMode.ElectronicCodeBook)
        {
            var initializationVector = ReadAllBytesFromFile(encryptionParams.InitializationVectorFileName);
            _symmetricCipherManager.InitializationVector = initializationVector;
        }

        var processedData = encryptionParams.CipherAction switch
        {
            CipherAction.Encrypt => _symmetricCipherManager.Encrypt(dataToProcess),
            CipherAction.Decrypt => _symmetricCipherManager.Decrypt(dataToProcess)
        };

        WriteAllBytesToFile(encryptionParams.OutputFileName, processedData);
    }

    public async Task<byte[]> HandleEncryption(CipherAction cipherAction, SymmetricCipherMode cipherMode,
        CipherBlockSize cipherBlockSize,
        Stream data, Stream keyStream, Stream initilizationVectorStream)
    {
        var dataToProcess = await ReadAllBytesFromStream(data);
        var key = await ReadAllBytesFromStream(keyStream);

        _symmetricCipherManager.Key = key;
        _symmetricCipherManager.CipherMode = cipherMode;
        _symmetricCipherManager.CipherBlockSize = cipherBlockSize;

        if (cipherMode is not SymmetricCipherMode.ElectronicCodeBook)
        {
            var initializationVector = await ReadAllBytesFromStream(initilizationVectorStream);
            _symmetricCipherManager.InitializationVector = initializationVector;
        }

        var processedData = cipherAction switch
        {
            CipherAction.Encrypt => _symmetricCipherManager.Encrypt(dataToProcess),
            CipherAction.Decrypt => _symmetricCipherManager.Decrypt(dataToProcess)
        };

        return processedData;
    }

    public void GenerateAndSaveRandomKeyToFile(string fileName, CipherBlockSize cipherBlockSize)
    {
        var randomKey = GenerateRandomKey((int)cipherBlockSize / 8);
        WriteAllBytesToFile(fileName, randomKey);
    }

    #endregion

    #region Utils

    private static byte[] GenerateRandomKey(int keyLength)
    {
        var random = new Random();
        var randomKey = new byte[keyLength];
        random.NextBytes(randomKey);
        return randomKey;
    }

    private byte[] ReadAllBytesFromFile(string fileName)
    {
        using var SourceStream = File.OpenRead(fileName);
        var fileData = new byte[SourceStream.Length];
        SourceStream.Read(fileData, 0, (int)SourceStream.Length);
        return fileData;
    }

    private async Task<byte[]> ReadAllBytesFromStream(Stream stream)
    {
        var fileData = new byte[stream.Length];
        await stream.ReadAsync(fileData, 0, (int)stream.Length);
        stream.Dispose();
        return fileData;
    }

    private void WriteAllBytesToFile(string fileName, byte[] data)
    {
        using var fileStream = File.OpenWrite(fileName);
        fileStream.Write(data);
    }

    #endregion
}