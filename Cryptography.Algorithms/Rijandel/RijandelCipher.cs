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

        #region Methods

        public byte[] Encrypt(byte[] dataBytes)
        {
            if (dataBytes.Length != 16)
            {
                throw new ArgumentException("dataBytes");
            }
            var state = (byte[])dataBytes.Clone();
            AddRoundKey(state, Key);
            for (var round = 0; round < RoundsCount - 1; round++)
            {
                SubBytes(state);
                ShiftRows(state);
                MixColumns(state);
                AddRoundKey(state, _roundKeys[round]);
            }
            SubBytes(state);
            ShiftRows(state);
            AddRoundKey(state, _roundKeys[RoundsCount - 1]);
            return state;
        }

        public byte[] Decrypt(byte[] dataBytes)
        {
            if (dataBytes.Length != 16)
            {
                throw new ArgumentException("dataBytes");
            }
            var state = (byte[])dataBytes.Clone();
            AddRoundKey(state, _roundKeys.Last());
            for (var round = 0; round < RoundsCount - 1; round++)
            {
                InversedShiftRows(state);
                InversedSubBytes(state);
                AddRoundKey(state, _roundKeys[RoundsCount - round - 2]);
                InversedMixColumns(state);
            }
            InversedShiftRows(state);
            InversedSubBytes(state);
            AddRoundKey(state, Key);
            return state;
        }

        #endregion

        #region Secondary methods

        private void CreateRoundKeys()
        {
            RoundsCount = RijandelUtils.RijandelModes[(CipherBlockSize) Key.Length].CountRounds; 
            _roundKeys = new byte[RoundsCount][];
            for (var i = 0; i < RoundsCount; i++)
            {
                _roundKeys[i] = new byte[Key.Length];
                if (i == 0)
                {
                    byte[] W = new byte[4]
                    {
                        Key[Key.Length - 3],
                        Key[Key.Length - 2],
                        Key[Key.Length - 1],
                        Key[Key.Length - 4]
                    };
                    for (var j = 0; j < 4; j++)
                    {
                        _roundKeys[i][j] = (byte)(Key[j] ^ _sBox[W[j]] ^ (j == 0 ? _rCon[i % _rCon.Length] : 0));
                    }
                    for (var j = 0; j < 3; j++)
                    {
                        for (var k = 0; k < 4; k++)
                        {
                            _roundKeys[i][(j + 1) * 4 + k] = (byte)(Key[(j + 1) * 4 + k] ^ _roundKeys[i][j * 4 + k]);
                        }
                    }
                }
                else
                {
                    var W = new byte[4]
                    {
                        _roundKeys[i - 1][Key.Length - 3],
                        _roundKeys[i - 1][Key.Length - 2],
                        _roundKeys[i - 1][Key.Length - 1],
                        _roundKeys[i - 1][Key.Length - 4]
                    };
                    for (var j = 0; j < 4; j++)
                    {
                        _roundKeys[i][j] = (byte)(_roundKeys[i - 1][j] ^ _sBox[W[j]] ^ (j == 0 ? _sBox[i % _rCon.Length] : 0));
                    }
                    for (var j = 0; j < 3; j++)
                    {
                        for (var k = 0; k < 4; k++)
                        {
                            _roundKeys[i][(j + 1) * 4 + k] = (byte)(_roundKeys[i - 1][(j + 1) * 4 + k] ^ _roundKeys[i][j * 4 + k]);
                        }
                    }
                }
            }
        }

        private void AddRoundKey(byte[] state, byte[] roundkey)
        {
            for (var i = 0; i < state.Length; i++)
            {
                state[i] ^= roundkey[i];
            }
        }

        #region Secondary encryption functions

        private void SubBytes(byte[] state)
        {
            for (var i = 0; i < state.Length; i++)
            {
                state[i] = _sBox[state[i]];
            }
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

        private void MixColumns(byte[] state)
        {
            var temporaryState = new byte[state.Length];
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    temporaryState[i * 4 + j] =  _galoisField.Multiply(_cPolynomial[j * 4], state[i * 4]);
                    temporaryState[i * 4 + j] ^= _galoisField.Multiply(_cPolynomial[j * 4 + 1], state[i * 4 + 1]);
                    temporaryState[i * 4 + j] ^= _galoisField.Multiply(_cPolynomial[j * 4 + 2], state[i * 4 + 2]);
                    temporaryState[i * 4 + j] ^= _galoisField.Multiply(_cPolynomial[j * 4 + 3], state[i * 4 + 3]);
                }
            }
            for (var i = 0; i < state.Length; i++)
            {
                state[i] = temporaryState[i];
            }
        }

        #endregion

        #region Secondary decryption functions

        private void InversedSubBytes(byte[] state)
        {
            for (var i = 0; i < state.Length; i++)
            {
                state[i] = _inversedSBox[state[i]];
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

        private void InversedMixColumns(byte[] state)
        {
            var temporaryState = new byte[16];
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    temporaryState[i * 4 + j] = _galoisField.Multiply(_inversedCPolynomial[j * 4], state[j * 4]); 
                    temporaryState[i * 4 + j] ^=  _galoisField.Multiply(_inversedCPolynomial[j * 4 + 1], state[i * 4 + 1]);
                    temporaryState[i * 4 + j] ^=  _galoisField.Multiply(_inversedCPolynomial[j * 4 + 2], state[i * 4 + 2]);
                    temporaryState[i * 4 + j] ^=  _galoisField.Multiply(_inversedCPolynomial[j * 4 + 3], state[i * 4 + 3]);
                }
            }
            for (var i = 0; i < state.Length; i++)
            {
                state[i] = temporaryState[i];
            }
        }

        #endregion

        #endregion

        public byte[] Encrypt(byte[] openText, byte[] key)
        {
            Key = key;
            return Encrypt(openText, key);
        }

        public byte[] Decrypt(byte[] encryptedText, byte[] key)
        {
            Key = key;
            return Decrypt(encryptedText, key);
        }
    }

}