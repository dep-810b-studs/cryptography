using System;

namespace Cryptography.Algorithms.Symmetric
{
    internal interface ISymmetricCipherManager
    {
        byte[] Encrypt(byte[] message);
        byte[] Decrypt(byte[] message);
        
        byte[] Key { get; set; }
        byte[] InitializationVector { get; set; }
        SymmetricCipherMode CipherMode { get; set; }
        CipherBlockSize CipherBlockSize { get; set; }
    }

    internal class SymmetricCipherManager : ISymmetricCipherManager
    {
        #region Manager Data

        private readonly ISymmetricCipher _symmetricCipher;

        #endregion
        
        #region Manager API

        public SymmetricCipherManager(ISymmetricCipher symmetricCipher)
        {
            _symmetricCipher = symmetricCipher;
        }

        public byte[] Key { get; set; }
        public byte[] InitializationVector { get; set; }
        public SymmetricCipherMode CipherMode { get; set; }
        public CipherBlockSize CipherBlockSize { get; set; }
        
        public byte[] Encrypt(byte[] message)
        {
            switch (CipherMode)
            {
                case SymmetricCipherMode.ElectronicCodeBook:
                    break;
                case SymmetricCipherMode.CipherFeedback:
                    break;
                case SymmetricCipherMode.OutputFeedback:
                    break;
                case SymmetricCipherMode.CipherBlockChaining:
                    break;
            }
            return _symmetricCipher.Encrypt(message, Key);
        }

        public byte[] Decrypt(byte[] message)
        {
            switch (CipherMode)
            {
                case SymmetricCipherMode.ElectronicCodeBook:
                    break;
                case SymmetricCipherMode.CipherFeedback:
                    break;
                case SymmetricCipherMode.OutputFeedback:
                    break;
                case SymmetricCipherMode.CipherBlockChaining:
                    break;
            }
            return _symmetricCipher.Decrypt(message, Key);
        }

        #endregion

        #region Modes

        private byte[] ElectronicCodeBook()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}