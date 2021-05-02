using System.ComponentModel.DataAnnotations;
using Cryptography.Algorithms.Symmetric;

namespace Cryptography.WebInterface.SymmetricCipher
{
    public class CipherParamsModel
    {
        [Required]
        public CipherAction CipherAction { get; set; }
        [Required(ErrorMessage = "Cipher block size should be specified")]
        public CipherBlockSize CipherBlockSize { get; set; }
        [Required]
        public SymmetricCipherMode SymmetricCipherMode { get; set; }
        public string InputFileName { get; set; }
        public string OutputFileName { get; set; }
        public string KeyFileName { get; set; }
        public string InitializationVectorFileName { get; set; }
    }
}