using System;
using Cryptography.Algorithms;
using Cryptography.Algorithms.Rijandel;
using Cryptography.Algorithms.Symmetric;
using Cryptography.Algorithms.Symmetric.CipherManager;
using Cryptography.Algorithms.Symmetric.CipherSystem;
using Cryptography.Algorithms.Symmetric.Padding;

namespace Cryptography.DemoApplication.Jobs
{
    public class EncryptionAlgorithmJob : BaseJobs
    {
        private readonly ISymmetricSystem _symmetricSystem;

        protected virtual CipherBlockSize _cipherBlockSize { get;}

        public EncryptionAlgorithmJob(ISymmetricCipher symmetricCipher)
        {
            Actions = new Action[]
            {
                Encrypt,
                Decrypt,
                GenerateRandomKey
            };
            var paddingService = new PaddingService();
            var symmetricCipherManager = new SymmetricCipherManager(symmetricCipher, paddingService);
            _symmetricSystem = new SymmetricSystem(symmetricCipherManager);
        }

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
}