using System;
using System.Collections.Generic;
using System.Linq;
using Cryptography.Algorithms.Symmetric;
using Cryptography.Arithmetic.GaloisField;

namespace Cryptography.Algorithms.Rijandel
{
    internal interface IRijandelUtils
    {
        byte[] CreateSBox();
    }
    
    internal class RijandelUtils : IRijandelUtils
    {
        private readonly GaloisField _galoisField = new ();

        private readonly Dictionary<CipherBlockSize, int> _blockSizeCountBytes = new()
        {
            [CipherBlockSize.Small] = 16,
            [CipherBlockSize.Middle] = 24,
            [CipherBlockSize.Big] = 32,
        };
        
        public byte[] CreateSBox()
        {
            var result = new byte[256];
            foreach (byte i in Enumerable.Range(byte.MinValue, 256))
            {
                foreach (byte j in Enumerable.Range(byte.MinValue, 256))
                {
                    var MultiplicativeInversed = _galoisField.MultiplicativeInverse(i, MultiplicativeInverseCalculationWay.Exponentiation);
                    for (var k = 0; k < 5; k++)
                    {
                        result[i] ^= MultiplicativeInversed;
                        MultiplicativeInversed = (byte)((MultiplicativeInversed << 1) | (MultiplicativeInversed >> 7));
                    }
                    result[i] ^= 99;
                    break;
                    
                }
            };
            result[0] = 99;
            return result;
        }
        
        public byte[] GenerateRandomKey(CipherBlockSize cipherBlockSize)
        {
            var random = new Random();
            var keyLength = _blockSizeCountBytes[cipherBlockSize];
            var randomKey = new byte[keyLength];
            random.NextBytes(randomKey);
            return randomKey;
        }
        
    }
}