namespace Cryptography.Algorithms.RSA
{
    public class RSAEncryptionResult
    {
        public ulong CipherText { get; init; }
        public (ulong N, ulong E) PublicKey { get; init; }
        public (ulong d, uint p, uint q) SecretKey { get; init; }
    }
}