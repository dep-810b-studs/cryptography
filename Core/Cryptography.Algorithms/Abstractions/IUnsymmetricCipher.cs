namespace Cryptography.Algorithms;

public interface IUnSymmetricCipher
{
    ulong EnCrypt(ulong message, (ulong, ulong, ulong) key);
    ulong DeCrypt(ulong cipherText, (ulong, ulong) ket);
}