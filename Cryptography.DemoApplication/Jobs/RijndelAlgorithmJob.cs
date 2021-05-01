using System;
using System.Threading.Tasks;
using Cryptography.Algorithms.Rijandel;
using Cryptography.Algorithms.Symmetric;
using Cryptography.Algorithms.Symmetric.CipherManager;
using Cryptography.Algorithms.Symmetric.CipherSystem;
using Cryptography.Algorithms.Symmetric.Padding;

namespace Cryptography.DemoApplication.Jobs
{
    [Job("rijndel")]
    public class RijndelAlgorithmJob : BaseJobs
    {
        private readonly ISymmetricSystem _symmetricSystem;

        public RijndelAlgorithmJob()
        {
            Actions = new Action[]
            {
                Encrypt,
                Decrypt,
                GenerateRandomKey
            };
            var symmetricCipher = new RijandelCipher();
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

            var encryptionParams = new EncryptionParams(CipherAction.Encrypt, CipherBlockSize.Small,
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

            var encryptionParams = new EncryptionParams(CipherAction.Decrypt, CipherBlockSize.Small,
                SymmetricCipherMode.ElectronicCodeBook,
                inputFileName, outputFileName, keyFileName);
            
            _symmetricSystem.HandleEncryption(encryptionParams);
        }
        
        private void GenerateRandomKey()
        {
            Console.WriteLine("Введите имя файла");
            var fileName = Console.ReadLine();
            _symmetricSystem.GenerateAndSaveRandomKeyToFile(fileName, CipherBlockSize.Small);
        }
    }
}