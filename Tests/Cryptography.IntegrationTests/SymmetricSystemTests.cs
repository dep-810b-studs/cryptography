using System.Text;
using Cryptography.Algorithms.Rijandel;
using Cryptography.Algorithms.Symmetric;
using Cryptography.Algorithms.Symmetric.CipherManager;
using Cryptography.Algorithms.Symmetric.CipherStrategy;
using Cryptography.Algorithms.Symmetric.CipherSystem;
using Cryptography.Algorithms.Symmetric.Padding;

namespace Cryptography.IntegrationTests;

public class SymmetricSystemTests
{
    [Fact]
    public void Test()
    {
        var strategies = new Dictionary<SymmetricCipherMode, ICipherStrategy>()
        {
            [SymmetricCipherMode.ElectronicCodeBook] = new ElectronicCodeBookStrategy()
        };
        var systemUnderTest =
            new SymmetricSystem(new SymmetricCipherManager(new RijandelCipher(), new PaddingService(), strategies));

        var random = new Random();
        var key = new byte[16];
        random.NextBytes(key);

        var message = "Hello, world!Hello, world!Hello, world!Hello, world!Hello, world!Hello, world!Hello, world!";

        var encryptedMessage = systemUnderTest.HandleEncryption(CipherAction.Encrypt,
            SymmetricCipherMode.ElectronicCodeBook,
            CipherBlockSize.Small,
            Encoding.UTF8.GetBytes(message),
            key);

        var decryptedMessage = systemUnderTest.HandleEncryption(CipherAction.Decrypt,
            SymmetricCipherMode.ElectronicCodeBook,
            CipherBlockSize.Small,
            encryptedMessage,
            key);
        
        Assert.Equal(message, Encoding.UTF8.GetString(decryptedMessage));
    }
}