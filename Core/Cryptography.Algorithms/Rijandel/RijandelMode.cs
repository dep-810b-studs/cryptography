using Cryptography.Algorithms.Symmetric;

namespace Cryptography.Algorithms.Rijandel
{
    public record RijandelMode(CipherBlockSize CipherBlockSize, int BlockSizeCountBytes, byte CountRounds, (int C1,int C2, int C3) Shifts, int Nb);
}