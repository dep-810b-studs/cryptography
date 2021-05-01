using System;
using System.Threading.Tasks;
using Cryptography.Algorithms;
using Cryptography.Algorithms.Rijandel;
using Cryptography.Algorithms.Symmetric;
using Cryptography.Algorithms.Symmetric.CipherManager;
using Cryptography.Algorithms.Symmetric.CipherSystem;
using Cryptography.Algorithms.Symmetric.Padding;

namespace Cryptography.DemoApplication.Jobs
{
    [Job("rijndel")]
    public class RijndelAlgorithmJob : EncryptionAlgorithmJob
    {
        protected override CipherBlockSize _cipherBlockSize => CipherBlockSize.Small;
        public RijndelAlgorithmJob() : base(new RijandelCipher()) { }
    }
}