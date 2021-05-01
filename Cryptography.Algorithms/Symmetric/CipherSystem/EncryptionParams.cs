namespace Cryptography.Algorithms.Symmetric.CipherSystem
{
    public record EncryptionParams(CipherAction CipherAction,
        CipherBlockSize CipherBlockSize,
        SymmetricCipherMode SymmetricCipherMode,
        string InputFileName,
        string OutputFileName,
        string KeyFileName,
        string? InitializationVectorFileName = null);
}