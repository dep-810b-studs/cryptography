using System;
using System.Collections.Generic;
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
        Actions = new[]
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
        Console.WriteLine("Введите имя файла, который нужно сохранить");
        var outputFileName = Console.ReadLine();
        Console.WriteLine("Введите имя файла ключа, который нужно сохранить");
        var keyFileName = Console.ReadLine();

        var encryptionParams = new EncryptionParams(CipherAction.Encrypt, _cipherBlockSize,
            SymmetricCipherMode.ElectronicCodeBook,
            inputFileName, outputFileName, keyFileName);

        _symmetricSystem.HandleEncryption(encryptionParams);
    }

    private void Decrypt()
    {
        Console.WriteLine("Введите имя файла для расшифровки");
        var inputFileName = Console.ReadLine();
        Console.WriteLine("Введите имя файла, который нужно сохранить");
        var outputFileName = Console.ReadLine();
        Console.WriteLine("Введите имя файла ключа, который нужно сохранить");
        var keyFileName = Console.ReadLine();

        var encryptionParams = new EncryptionParams(CipherAction.Decrypt, _cipherBlockSize,
            SymmetricCipherMode.ElectronicCodeBook,
            inputFileName, outputFileName, keyFileName);

        _symmetricSystem.HandleEncryption(encryptionParams);
    }

    private void GenerateRandomKey()
    {
        Console.WriteLine("Введите имя файла");
        var fileName = Console.ReadLine();
        _symmetricSystem.GenerateAndSaveRandomKeyToFile(fileName, _cipherBlockSize);
    }
}