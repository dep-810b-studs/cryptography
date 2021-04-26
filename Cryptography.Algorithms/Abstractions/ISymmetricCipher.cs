namespace Cryptography.Algorithms
{
    public interface ISymmetricCipher
    {
        byte[] Encrypt(byte[] openText, byte[] key);
        byte[] Decrypt(byte[] encryptedText, byte[] key);
    }
}