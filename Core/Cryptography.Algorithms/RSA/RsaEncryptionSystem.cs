using System;
using System.Linq;
using Cryptography.Algorithms.Utils;
using Cryptography.Arithmetic.ResidueNumberSystem;

namespace Cryptography.Algorithms.RSA;

public interface IRsaEncryptionSystem
{
    (string, ulong) Encrypt(string message, string P, string Q, string e);
    string Decrypt(string message, string N, string D);
    ulong GetRandomPrimeNumber(int max, Func<int, bool> isNumberValid);
}

public class RsaEncryptionSystem : IRsaEncryptionSystem
{
    private readonly IMessageConvertor _messageConvertor;
    private readonly Random _random = new();
    private readonly IResidueNumberSystem _residueNumberSystem;
    private readonly IRSACipher _rsaCipher;

    public RsaEncryptionSystem(IMessageConvertor messageConvertor, IRSACipher rsaCipher,
        IResidueNumberSystem residueNumberSystem)
    {
        _messageConvertor = messageConvertor;
        _rsaCipher = rsaCipher;
        _residueNumberSystem = residueNumberSystem;
    }

    public (string, ulong) Encrypt(string message, string P, string Q, string e)
    {
        var convertedMessage = _messageConvertor.ConvertToLong(message);
        var convertedP = uint.Parse(P);
        var convertedQ = uint.Parse(Q);
        var convertedE = ulong.Parse(e);
        var encryptedMessage = _rsaCipher.EnCrypt(convertedMessage,
            convertedP, convertedQ, convertedE);

        return (encryptedMessage.CipherText.ToString(), encryptedMessage.SecretKey.d);
    }

    public string Decrypt(string message, string N, string D)
    {
        var convertedMessage = ulong.Parse(message);
        var convertedN = uint.Parse(N);
        var convertedD = ulong.Parse(D);

        var encryptionResult = new RSAEncryptionResult
        {
            CipherText = convertedMessage,
            PublicKey = (convertedN, 0),
            SecretKey = (convertedD, 0, 0)
        };

        var decryptedMessage = _rsaCipher.DeCrypt(encryptionResult);

        return _messageConvertor.ConvertToString(decryptedMessage);
    }

    public ulong GetRandomPrimeNumber(int max, Func<int, bool> isNumberValid)
    {
        var primeNumbers = _residueNumberSystem.GetSimpleNumbersLessThenM(max).ToArray();

        while (true)
        {
            var randomPrime = primeNumbers[_random.Next(primeNumbers.Length - 1)];
            if (isNumberValid(randomPrime))
                return (ulong)randomPrime;
        }
    }
}