namespace Cryptography.Algorithms
{
    public interface ISymmetricCipher
    {
        byte[] EnCrypt(byte[] openText, byte[] key);
        byte[] DeCrypt(byte[] encryptedText, byte[] key);
    }
}