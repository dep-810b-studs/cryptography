using System.Linq;
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
    }
}