using System;
using System.IO;
using System.Threading.Tasks;
using Cryptography.Algorithms.Symmetric.CipherManager;
using Cryptography.Algorithms.Utils;

namespace Cryptography.Algorithms.Symmetric.CipherSystem
{
    public interface ISymmetricSystem
    {
        void HandleEncryption(EncryptionParams encryptionParams);
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
            
            var processedData = new byte[]{};
            
            switch (encryptionParams.CipherAction)
            {
                case CipherAction.Encrypt:
                    processedData = _symmetricCipherManager.Encrypt(dataToProcess);
                    break;
                case CipherAction.Decrypt:
                    processedData = _symmetricCipherManager.Decrypt(dataToProcess);
                    break;
            }

            WriteAllBytesToFile(encryptionParams.OutputFileName, processedData);
        }
        
        public void GenerateAndSaveRandomKeyToFile(string fileName, CipherBlockSize cipherBlockSize)
        {
            var randomKey = GenerateRandomKey((int) cipherBlockSize / 8);
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
            using FileStream SourceStream = File.OpenRead(fileName);
            var fileData = new byte[SourceStream.Length];
            SourceStream.Read(fileData, 0, (int)SourceStream.Length);
            return fileData;
        }

        private void WriteAllBytesToFile(string fileName, byte[] data)
        {
            using FileStream fileStream = File.OpenWrite(fileName);
            fileStream.Write(data);
        }

        #endregion

    }
}