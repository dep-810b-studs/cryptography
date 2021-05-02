using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Cryptography.Algorithms.Symmetric.CipherStrategy;
using Cryptography.Algorithms.Symmetric.Padding;

namespace Cryptography.Algorithms.Symmetric.CipherManager
{
    public interface ISymmetricCipherManager
    {
        byte[] Encrypt(byte[] message);
        byte[] Decrypt(byte[] message);
        
        byte[] Key { get; set; }
        byte[] InitializationVector { get; set; }
        SymmetricCipherMode CipherMode { get; set; }
        CipherBlockSize CipherBlockSize { get; set; }
    }

    public class SymmetricCipherManager : ISymmetricCipherManager
    {
        #region Manager Data

        private readonly ISymmetricCipher _symmetricCipher;
        private readonly IPaddingService _paddingService;
        private CipherBlockSize _cipherBlockSize;

        private readonly Dictionary<SymmetricCipherMode, ICipherStrategy> _cipherStrategies;

        private readonly Dictionary<CipherBlockSize, int> _cipherModes = new()
        {
            [CipherBlockSize.Des] = 8,
            [CipherBlockSize.Small] = 16,
            [CipherBlockSize.Middle] = 24,
            [CipherBlockSize.Big] = 32,
        };
        
        #endregion
        
        #region Manager API
        public SymmetricCipherManager(ISymmetricCipher symmetricCipher, IPaddingService paddingService, 
            Dictionary<SymmetricCipherMode, ICipherStrategy> cipherStrategies)
        {
            _paddingService = paddingService;
            _cipherStrategies = cipherStrategies;
            _symmetricCipher = symmetricCipher;
        }

        public byte[] Key { get; set; }
        public byte[] InitializationVector { get; set; }
        public SymmetricCipherMode CipherMode { get; set; }

        public CipherBlockSize CipherBlockSize
        {
            get => _cipherBlockSize;
            set
            {
                _symmetricCipher.CipherBlockSize = value;
                _cipherBlockSize = value;
            }
        }
        
        public byte[] Encrypt(byte[] message)
        {
            //todo: подумать над адекватной проверкой инициализации полей
            
            var blockSizeInBytes = _cipherModes[CipherBlockSize];
            var messageBlocks = GroupBytesByBlocks(message, blockSizeInBytes);

            if (message.Length % blockSizeInBytes != 0)
            {
                var entireBytesCount = messageBlocks.Count * blockSizeInBytes;
                var dataToPadding = message[entireBytesCount..];
                var (paddedBlock, paddingInfo) = _paddingService.FillEmptyBytes(dataToPadding, blockSizeInBytes);
                messageBlocks.Add(paddedBlock);
                messageBlocks.Add(paddingInfo);
            }

            _symmetricCipher.CreateRoundKeys(Key);
            var cipherText = _cipherStrategies[CipherMode].Encrypt(_symmetricCipher, messageBlocks);
            return cipherText.SelectMany(block => block).ToArray();
        }

        public byte[] Decrypt(byte[] message)
        {
            var blockSizeInBytes = _cipherModes[CipherBlockSize];

            if (message.Length % blockSizeInBytes != 0)
                throw new ArgumentOutOfRangeException(nameof(message));

            var messageBlocks = GroupBytesByBlocks(message, blockSizeInBytes);

            _symmetricCipher.CreateRoundKeys(Key);
            var openText = _cipherStrategies[CipherMode].Decrypt(_symmetricCipher, messageBlocks);
            _paddingService.RemovePaddedBytes(openText, blockSizeInBytes);
            return openText.SelectMany(block => block).ToArray();
        }

        #endregion

        private List<byte[]> GroupBytesByBlocks(byte[] data, int blockSizeInBytes)
        {
            var dataBlocks = new List<byte[]>();
            
            for (int blockNumber = 0; blockNumber < data.Length / blockSizeInBytes; blockNumber++)
            {
                var currentBlock = new byte[blockSizeInBytes];
                    
                for (int byteNumber = 0; byteNumber < blockSizeInBytes; byteNumber++)
                {
                    var offset = blockNumber * blockSizeInBytes;
                    currentBlock[byteNumber] = data[byteNumber + offset];
                }    
                
                dataBlocks.Add(currentBlock);
            }

            return dataBlocks;
        }
    }
}
