using Cryptography.Algorithms.DES;
using Cryptography.Algorithms.Symmetric;

namespace Cryptography.DemoApplication.Jobs
{
    [Job("des")]
    public class DesAlgorithmJob : EncryptionAlgorithmJob
    {
        protected override CipherBlockSize _cipherBlockSize => CipherBlockSize.Des;
        public DesAlgorithmJob() : base(new DesCipher()) { }
    }
}