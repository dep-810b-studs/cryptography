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
        private static readonly byte[] _cPolynomial = new byte[] { 2, 3, 1, 1, 1, 2, 3, 1, 1, 1, 2, 3, 3, 1, 1, 2 };
        private static byte[] _inversedCPolynomial = new byte[] { 14, 11, 13, 9, 9, 14, 11, 13, 13, 9, 14, 11, 11, 13, 9, 14 };
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

        public RijandelCipher()
        {
            Key = new byte[16];
        }
        
        public byte[] Key
        {
            private get => _key;

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Key");
                }
                if (!(new[] { 16, 24, 32 }).Contains(value.Length))
                {
                    throw new ArgumentException("Key");
                }
                _key = (byte[])value.Clone();
                CreateRoundKeys();
            }
        }

        private byte RoundsCount
        {
            get => _roundsCount;

            set
            {
                var supportedRoundsCount = RijandelUtils.RijandelModes.Select((mode) => mode.Value.CountRounds);

                if (!supportedRoundsCount.Contains(value))
                    throw new ArgumentOutOfRangeException(nameof(RoundsCount));

                _roundsCount = value;
            }
        }
        
        public byte[] EncryptionConvertion(byte[] data, CipherAction cipherAction)
        {
            //todo: добавить проверку на количество бит 128/196/256
            
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
            RoundsCount = RijandelUtils.RijandelModes[(CipherBlockSize) Key.Length].CountRounds; 
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

        private void ShiftRows(byte[] state)
        {
            var temporary = state[1];
            state[1] = state[5];
            state[5] = state[9];
            state[9] = state[13];
            state[13] = temporary;
            temporary = state[2];
            state[2] = state[10];
            state[10] = temporary;
            temporary = state[6];
            state[6] = state[14];
            state[14] = temporary;
            temporary = state[3];
            state[3] = state[15];
            state[15] = state[11];
            state[11] = state[7];
            state[7] = temporary;
        }

        private void MixColumns(byte[] state, CipherAction cipherAction)
        {
            var cPolynomial = cipherAction switch
            {
                CipherAction.Encrypt => _cPolynomial,
                CipherAction.Decrypt => _inversedCPolynomial
            };
            
            var temporaryState = new byte[state.Length];
            
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    temporaryState[i * 4 + j] =  _galoisField.Multiply(cPolynomial[j * 4], state[i * 4]);
                    temporaryState[i * 4 + j] ^= _galoisField.Multiply(cPolynomial[j * 4 + 1], state[i * 4 + 1]);
                    temporaryState[i * 4 + j] ^= _galoisField.Multiply(cPolynomial[j * 4 + 2], state[i * 4 + 2]);
                    temporaryState[i * 4 + j] ^= _galoisField.Multiply(cPolynomial[j * 4 + 3], state[i * 4 + 3]);
                }
            }
            
            for (var i = 0; i < state.Length; i++)
            {
                state[i] = temporaryState[i];
            }
        }
        
        private void InversedShiftRows(byte[] state)
        {
            var temporary = state[1];
            state[1] = state[13];
            state[13] = state[9];
            state[9] = state[5];
            state[5] = temporary;
            temporary = state[2];
            state[2] = state[10];
            state[10] = temporary;
            temporary = state[6];
            state[6] = state[14];
            state[14] = temporary;
            temporary = state[3];
            state[3] = state[7];
            state[7] = state[11];
            state[11] = state[15];
            state[15] = temporary;
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
            ShiftRows(state);
            MixColumns(state, CipherAction.Encrypt);
            AddRoundKey(state, _roundKeys[roundNumber]);
        }
        
        private void DecryptionRound(byte[] state, int roundNumber)
        {
            InversedShiftRows(state);
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
                    ShiftRows(state);
                    AddRoundKey(state, _roundKeys.Last());
                    break;
                case CipherAction.Decrypt:
                    InversedShiftRows(state);
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
            return EncryptionConvertion(cipherText, CipherAction.Encrypt); 
        }
    }

}