namespace Cryptography.Algorithms.Symmetric
{
    internal interface ISymmetricCipherManager
    {
        byte[] Encrypt(byte[] message, byte[] key, SymmetricCipherMode cipherMode);
        byte[] Decrypt(byte[] message, byte[] key, SymmetricCipherMode cipherMode);
    }

    internal class SymmetricCipherManager : ISymmetricCipherManager
    {
        public byte[] Encrypt(byte[] message, byte[] key, SymmetricCipherMode cipherMode)
        {
            throw new System.NotImplementedException();
        }

        public byte[] Decrypt(byte[] message, byte[] key, SymmetricCipherMode cipherMode)
        {
            throw new System.NotImplementedException();
        }
    }
}