using System;
using System.Collections.Generic;
using System.Linq;
using Cryptography.Algorithms.Symmetric;
using Cryptography.Arithmetic.GaloisField;

namespace Cryptography.Algorithms.Rijandel
{
    internal class RijandelUtils
    {
        private static readonly GaloisField _galoisField = new ();

        public static readonly Dictionary<CipherBlockSize, RijandelMode> RijandelModes = new()
        {
            [CipherBlockSize.Small] = new (CipherBlockSize.Small, 16,10),
            [CipherBlockSize.Middle] = new (CipherBlockSize.Small, 24,12),
            [CipherBlockSize.Big] = new (CipherBlockSize.Small, 32,14),
        };
        
        public static byte[] CreateSBox()
        {
            var result = new byte[256];
            
            for (int i = 1; i < 256; i++)
            {
                var MultiplicativeInversed = _galoisField.MultiplicativeInverse((byte)i, MultiplicativeInverseCalculationWay.Exponentiation);
                
                for (var k = 0; k < 5; k++)
                {
                    result[i] ^= MultiplicativeInversed;
                    MultiplicativeInversed = (byte)((MultiplicativeInversed << 1) | (MultiplicativeInversed >> 7));
                }
                
                result[i] ^= 99;
            }
            
            result[0] = 0x63;
            return result;
        }

        public static byte[] CreateInversedSBox(byte[] sBox = null)
        {
            sBox ??= CreateSBox();
            var insersedSBox = new byte[sBox.Length];

            for (int i = 0; i < sBox.Length; i++)
            {
                insersedSBox[sBox[i]] = (byte)i;
            }

            return insersedSBox;
        }

        public static byte[] GenerateRandomKey(CipherBlockSize cipherBlockSize)
        {
            var random = new Random();
            var keyLength = RijandelModes[cipherBlockSize].BlockSizeCountBytes;
            var randomKey = new byte[keyLength];
            random.NextBytes(randomKey);
            return randomKey;
        }
        
    }
}