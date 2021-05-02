using Cryptography.Algorithms.Symmetric;

namespace Cryptography.Algorithms
{
    public interface ISymmetricCipher
    {
        CipherBlockSize CipherBlockSize { get; set; }
        byte[] Encrypt(byte[] openText);
        byte[] Decrypt(byte[] encryptedText);
        void CreateRoundKeys(byte[] key);
    }
}