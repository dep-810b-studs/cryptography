using System;
using System.Linq;
using Cryptography.Algorithms.Symmetric;
using Cryptography.Arithmetic.GaloisField;

namespace Cryptography.Algorithms.Rijandel
{

    public sealed class RijandelCipher : ISymmetricCipher
    {
        private static readonly byte[] _sBox;
        private static readonly byte[] _inversedSBox;
        private static readonly byte[] _cPolynomial = new byte[] { 3, 1 ,1, 2 };
        private static byte[] _inversedCPolynomial = new byte[] { 11, 13, 9, 14};
        private static byte[] _rCon = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128, 27, 54 };
        private static readonly GaloisField _galoisField = new (); 

        private byte[] _key;
        private byte[][] _roundKeys;
        private byte _roundsCount;

        static RijandelCipher()
        {
            _sBox = RijandelUtils.CreateSBox();
            _inversedSBox = RijandelUtils.CreateInversedSBox(_sBox);
        }

        public byte[] Key
        {
            private get => _key;

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException("Key");
                }

                if (!RijandelUtils.SuppotedBlockSizeCountByte.Contains((byte)value.Length))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                
                _key = value.Clone() as byte[];
                CreateRoundKeys();
            }
        }

        private byte RoundsCount
        {
            get => _roundsCount;

            set
            {
                if (!RijandelUtils.SuppotedCountRounds.Contains(value))
                    throw new ArgumentOutOfRangeException(nameof(RoundsCount));

                _roundsCount = value;
            }
        }

        private RijandelMode Mode => RijandelUtils.RijandelModes[CipherBlockSize];

        private const int RowsCount = 4;
        
        public CipherBlockSize CipherBlockSize { get; set; } 
            
        private byte[] EncryptionConvertion(byte[] data, CipherAction cipherAction)
        {
            if (!RijandelUtils.SuppotedBlockSizeCountByte.Contains(data.Length))
                throw new ArgumentOutOfRangeException(nameof(data));
            
            var state = data.Clone() as byte[];
            
            InitialRound(state, cipherAction);

            Action<byte[],int> round = cipherAction switch
            {
                CipherAction.Encrypt => EncryptionRound,
                CipherAction.Decrypt => DecryptionRound
            };
            
            for (var roundNumber = 0; roundNumber < RoundsCount - 1; roundNumber++)
            {
                round(state, roundNumber);
            }
            
            FinalRound(state, cipherAction);

            return state;
        }

        private void CreateRoundKeys()
        {
            RoundsCount = RijandelUtils.RijandelModes[CipherBlockSize].CountRounds; 
            _roundKeys = new byte[RoundsCount][];
            
            for (var i = 0; i < RoundsCount; i++)
            {
                _roundKeys[i] = new byte[Key.Length];

                var sourceKey = i == 0 ? Key : _roundKeys[i - 1];
                
                byte[] W = new byte[4]
                {
                    sourceKey[Key.Length - 3],
                    sourceKey[Key.Length - 2],
                    sourceKey[Key.Length - 1],
                    sourceKey[Key.Length - 4]
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
            
            state = stateCopy.Select((_, index) => substitutionTable[stateCopy[index]]).ToArray();
        }
        
        private void AddRoundKey(byte[] state, byte[] roundkey)
        {
            state = state.Select((number, index) => (byte)(number ^ roundkey[index])).ToArray();
        }
        

        public void ShiftRowsUsingMacros(byte[] state, CipherAction cipherAction)
        {
            var (c1, c2, c3) = Mode.Shifts;
            var countBytesInRow = Mode.Nb;

            if (cipherAction is CipherAction.Decrypt)
            {
                (c1, c2, c3) = (countBytesInRow - c1, countBytesInRow - c2, countBytesInRow - c3);
            }

            var row1 = RijandelUtils.GetRow(state, 1, countBytesInRow);
            RijandelUtils.CyclicShift(row1, c1);
            RijandelUtils.SetRow(state,row1,1,countBytesInRow);
            
            var row2 = RijandelUtils.GetRow(state, 2, countBytesInRow);
            RijandelUtils.CyclicShift(row2, c2);
            RijandelUtils.SetRow(state,row2,2,countBytesInRow);
            
            var row3 = RijandelUtils.GetRow(state, 3, countBytesInRow);
            RijandelUtils.CyclicShift(row3, c3);
            RijandelUtils.SetRow(state,row3,3,countBytesInRow );
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
            for (int index = 0; index < blockSize; ++index)
                state[index] = numArray[index];
        }

        #region Rounds

        private void InitialRound(byte[] state, CipherAction cipherAction)
        {
            var key = cipherAction switch
            {
                CipherAction.Encrypt => Key,
                CipherAction.Decrypt => _roundKeys.Last()
            };
            
            AddRoundKey(state, key);
        }
        
        private void EncryptionRound(byte[] state, int roundNumber)
        {
            SubBytes(state, CipherAction.Encrypt);
            ShiftRowsUsingMacros(state, CipherAction.Encrypt);
            MixColumns(state, CipherAction.Encrypt);
            AddRoundKey(state, _roundKeys[roundNumber]);
        }
        
        private void DecryptionRound(byte[] state, int roundNumber)
        {
            ShiftRowsUsingMacros(state, CipherAction.Decrypt);
            SubBytes(state, CipherAction.Decrypt);
            AddRoundKey(state, _roundKeys[RoundsCount - roundNumber - 2]);
            MixColumns(state, CipherAction.Decrypt);
        }
        
        private void FinalRound(byte[] state, CipherAction cipherAction)
        {
            switch (cipherAction)
            {
                case CipherAction.Encrypt:
                    SubBytes(state, CipherAction.Encrypt);
                    //ShiftRows(state);
                    ShiftRowsUsingMacros(state, CipherAction.Encrypt);
                    AddRoundKey(state, _roundKeys.Last());
                    break;
                case CipherAction.Decrypt:
                    //InversedShiftRows(state);
                    ShiftRowsUsingMacros(state, CipherAction.Decrypt);
                    SubBytes(state, CipherAction.Decrypt);
                    AddRoundKey(state, Key);
                    break;
                    
            }

        }

        #endregion
        
        public byte[] Encrypt(byte[] openText, byte[] key)
        {
            Key = key;
            return EncryptionConvertion(openText, CipherAction.Encrypt);
        }

        public byte[] Decrypt(byte[] cipherText, byte[] key)
        {
            Key = key;
            return EncryptionConvertion(cipherText, CipherAction.Decrypt); 
        }
    }

}