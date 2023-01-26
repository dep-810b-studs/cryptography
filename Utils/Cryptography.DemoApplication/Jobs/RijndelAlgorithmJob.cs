using Cryptography.Algorithms.Rijandel;
using Cryptography.Algorithms.Symmetric;

namespace Cryptography.DemoApplication.Jobs;

[Job("rijndel")]
public class RijndelAlgorithmJob : EncryptionAlgorithmJob
{
    public RijndelAlgorithmJob() : base(new RijandelCipher())
    {
    }

    protected override CipherBlockSize _cipherBlockSize => CipherBlockSize.Small;
}