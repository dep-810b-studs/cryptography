using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptography.Algorithms.Symmetric;
using Cryptography.Arithmetic.GaloisField;

namespace Cryptography.Algorithms.Rijandel
{
    public sealed class RijandelCipher : ISymmetricCipher
    {
        #region Algorithm Data

        private static readonly byte[] _sBox;
        private static readonly byte[] _inversedSBox;
        private static readonly byte[] _cPolynomial = new byte[] { 3, 1 ,1, 2 };
        private static byte[] _inversedCPolynomial = new byte[] { 11, 13, 9, 14};
        private static byte[] _rCon = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128, 27, 54 };
        private static readonly GaloisField _galoisField = new ();
        private byte[][] _roundKeys;
        private byte _roundsCount;
        private RijandelMode Mode => RijandelUtils.RijandelModes[CipherBlockSize];


        static RijandelCipher()
        {
            _sBox = RijandelUtils.CreateSBox();
            _inversedSBox = RijandelUtils.CreateInversedSBox(_sBox);
        }

        #endregion

        #region API of encryption

        public CipherBlockSize CipherBlockSize { get; set; }

        public byte[] Encrypt(byte[] openText) => EncryptionConvertion(openText, CipherAction.Encrypt);
        public byte[] Decrypt(byte[] cipherText) => EncryptionConvertion(cipherText, CipherAction.Decrypt);

        #endregion

        #region Main algorithm

        private byte[] EncryptionConvertion(byte[] data, CipherAction cipherAction)
        {
            if (!RijandelUtils.SuppotedBlockSizeCountByte.Contains(data.Length))
                throw new ArgumentOutOfRangeException(nameof(data));
            
            var state = data.Clone() as byte[];
            
            InitialRound(state, cipherAction);
            
            for (var roundNumber = 0; roundNumber < Mode.CountRounds - 1; roundNumber++)
            {
                Round(state, roundNumber, cipherAction);
            }
            
            FinalRound(state, cipherAction);

            return state;
        }

        #endregion

        #region Rounds

        private void InitialRound(byte[] state, CipherAction cipherAction)
        {
            var key = cipherAction switch
            {
                CipherAction.Encrypt => _roundKeys.First(),
                CipherAction.Decrypt => _roundKeys.Last()
            };
            
            AddRoundKey(state, key);
        }

        private void Round(byte[] state, int roundNumber, CipherAction cipherAction)
        {
            switch (cipherAction)
            {
                case CipherAction.Encrypt:
                    SubBytes(state, CipherAction.Encrypt);
                    ShiftRows(state, CipherAction.Encrypt);
                    MixColumns(state, CipherAction.Encrypt);
                    AddRoundKey(state, _roundKeys[roundNumber]);
                    break;
                case CipherAction.Decrypt:
                    ShiftRows(state, CipherAction.Decrypt);
                    SubBytes(state, CipherAction.Decrypt);
                    AddRoundKey(state, _roundKeys[Mode.CountRounds - roundNumber - 2]);
                    MixColumns(state, CipherAction.Decrypt);
                    break;
            }
        }

        private void FinalRound(byte[] state, CipherAction cipherAction)
        {
            switch (cipherAction)
            {
                case CipherAction.Encrypt:
                    SubBytes(state, CipherAction.Encrypt);
                    ShiftRows(state, CipherAction.Encrypt);
                    AddRoundKey(state, _roundKeys.Last());
                    break;
                case CipherAction.Decrypt:
                    ShiftRows(state, CipherAction.Decrypt);
                    SubBytes(state, CipherAction.Decrypt);
                    AddRoundKey(state, _roundKeys.First());
                    break;
            }
        }
        
        #endregion

        #region Round operations

        public void CreateRoundKeys(byte[] key)
        {
            _roundKeys = new byte[Mode.CountRounds][];
            
            for (var i = 0; i < Mode.CountRounds; i++)
            {
                _roundKeys[i] = new byte[key.Length];

                var sourceKey = i == 0 ? key : _roundKeys[i - 1];
                
                byte[] W = new byte[4]
                {
                    sourceKey[key.Length - 3],
                    sourceKey[key.Length - 2],
                    sourceKey[key.Length - 1],
                    sourceKey[key.Length - 4]
                };
                    
                for (var j = 0; j < 4; j++)
                {
                    _roundKeys[i][j] = (byte)(sourceKey[j] ^ _sBox[W[j]] ^ (j == 0 ? _rCon[i % _rCon.Length] : 0));
                }
                
                for (var j = 0; j < 3; j++)
                {
                    for (var k = 0; k < 4; k++)
                    {
                        _roundKeys[i][(j + 1) * 4 + k] = (byte)(sourceKey[(j + 1) * 4 + k] ^ _roundKeys[i][j * 4 + k]);
                    }
                }
            }
        }
        private void SubBytes(byte[] state, CipherAction cipherAction)
        {
            var substitutionTable = cipherAction switch
            {
                CipherAction.Encrypt => _sBox,
                CipherAction.Decrypt => _inversedSBox
            };

            var stateCopy = state.Clone() as byte[];

            Parallel.For(0, state.Length, index => state[index] = substitutionTable[stateCopy[index]]);
        }
        private void AddRoundKey(byte[] state, byte[] roundkey)
        {
            Parallel.For(0, state.Length, index => state[index] = (byte) (state[index] ^ roundkey[index]));
        }
        
        public void ShiftRows(byte[] state, CipherAction cipherAction)
        {
            var shifts = new int[] {Mode.Shifts.C1, Mode.Shifts.C2, Mode.Shifts.C3};
            
            var countBytesInRow = Mode.Nb;

            Parallel.For(0, shifts.Length, index =>
            {
                if (cipherAction is CipherAction.Decrypt)
                    shifts[index] = countBytesInRow - shifts[index];

                var rowNumber = index + 1;
                
                var row = RijandelUtils.GetRow(state, rowNumber, countBytesInRow);
                RijandelUtils.CyclicShift(row, shifts[index]);
                RijandelUtils.SetRow(state, row, rowNumber, countBytesInRow);
            });
        }
        private void MixColumns(byte[] state, CipherAction cipherAction)
        {
            var cPolynomial = cipherAction switch
            {
                CipherAction.Encrypt => _cPolynomial,
                CipherAction.Decrypt => _inversedCPolynomial
            };
            
            var blockSize = Mode.BlockSizeCountBytes;
            byte[] numArray = new byte[blockSize];
            for (int column = 0; column < Mode.Nb; ++column)
            {
                for (int row = 0; row < 4; ++row)
                {
                    for (int index3 = 0; index3 < 4; ++index3)
                    {
                        numArray[column * 4 + index3] = _galoisField.Add(numArray[column * 4 + index3], 
                            _galoisField.Multiply(cPolynomial[row], state[column * 4 + (3 + index3 - row) % 4]));
                    }
                }
            }

            Parallel.For(0, blockSize, index => state[index] = numArray[index]);
        }

        #endregion
    }

}