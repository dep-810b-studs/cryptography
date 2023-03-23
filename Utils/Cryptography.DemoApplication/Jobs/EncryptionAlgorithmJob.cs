using System;
using System.Collections.Generic;
using System.IO;
using Cryptography.Algorithms;
using Cryptography.Algorithms.Symmetric;
using Cryptography.Algorithms.Symmetric.CipherManager;
using Cryptography.Algorithms.Symmetric.CipherStrategy;
using Cryptography.Algorithms.Symmetric.CipherSystem;
using Cryptography.Algorithms.Symmetric.Padding;

namespace Cryptography.DemoApplication.Jobs;

public class EncryptionAlgorithmJob : BaseJobs
{
    private readonly ISymmetricSystem _symmetricSystem;

    public EncryptionAlgorithmJob(ISymmetricCipher symmetricCipher)
    {
        Actions = new Delegate[]
        {
            Encrypt,
            Decrypt,
            GenerateRandomKey
        };
        var paddingService = new PaddingService();
        var strategies = new Dictionary<SymmetricCipherMode, ICipherStrategy>
        {
            [SymmetricCipherMode.ElectronicCodeBook] = new ElectronicCodeBookStrategy()
        };

        var symmetricCipherManager = new SymmetricCipherManager(symmetricCipher, paddingService, strategies);
        _symmetricSystem = new SymmetricSystem(symmetricCipherManager);
    }

    protected virtual CipherBlockSize _cipherBlockSize { get; }

    private void Encrypt()
    {
        Console.WriteLine("Введите имя файла для шифрования");
        var inputFileName = Console.ReadLine();
        var inputFileBytes = ReadAllBytesFromFile(inputFileName);
        
        Console.WriteLine("Введите имя файла, который нужно сохранить");
        var outputFileName = Console.ReadLine();
        
        Console.WriteLine("Введите имя файла ключа, который нужно сохранить");
        var keyFileName = Console.ReadLine();
        var keyFileBytes = ReadAllBytesFromFile(keyFileName);

        var encryptedData = _symmetricSystem.HandleEncryption(CipherAction.Encrypt, 
            SymmetricCipherMode.ElectronicCodeBook, _cipherBlockSize, inputFileBytes, keyFileBytes);
        
        using var file = File.OpenWrite(outputFileName);
        file.Write(encryptedData);
    }

    private void Decrypt()
    {
        Console.WriteLine("Введите имя файла для расшифровки");
        var inputFileName = Console.ReadLine();
        var inputFileBytes = ReadAllBytesFromFile(inputFileName);
        
        Console.WriteLine("Введите имя файла, который нужно сохранить");
        var outputFileName = Console.ReadLine();
        
        Console.WriteLine("Введите имя файла ключа, который нужно сохранить");
        var keyFileName = Console.ReadLine();
        var keyFileBytes = ReadAllBytesFromFile(keyFileName);

        var decryptedData = _symmetricSystem.HandleEncryption(CipherAction.Decrypt, 
            SymmetricCipherMode.ElectronicCodeBook, _cipherBlockSize, inputFileBytes, keyFileBytes);
        
        using var file = File.OpenWrite(outputFileName);
        file.Write(decryptedData);
    }

    private void GenerateRandomKey()
    {
        Console.WriteLine("Введите имя файла");
        var fileName = Console.ReadLine();
        var key = _symmetricSystem.GenerateRandomKey(_cipherBlockSize);
        using var file = File.OpenWrite(fileName);
        file.Write(key);
    }
    
    private static byte[] ReadAllBytesFromFile(string fileName)
    {
        using var sourceStream = File.OpenRead(fileName);
        using var tempStream = new MemoryStream(); 
        sourceStream.CopyTo(tempStream);
        return tempStream.ToArray();
    }
}