using Cryptography.Algorithms.Symmetric;

namespace Cryptography.Algorithms.Rijandel
{
    public record RijandelMode(CipherBlockSize CipherBlockSize, int BlockSizeCountBytes, byte CountRounds);
}