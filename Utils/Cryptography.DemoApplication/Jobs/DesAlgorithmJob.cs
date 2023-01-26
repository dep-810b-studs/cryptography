using Cryptography.Algorithms.DES;
using Cryptography.Algorithms.Symmetric;

namespace Cryptography.DemoApplication.Jobs;

[Job("des")]
public class DesAlgorithmJob : EncryptionAlgorithmJob
{
    public DesAlgorithmJob() : base(new DesCipher())
    {
    }

    protected override CipherBlockSize _cipherBlockSize => CipherBlockSize.Des;
}