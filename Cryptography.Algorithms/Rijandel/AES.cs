
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AES
{
    public  class AES
    {
        #region Static fields

        private static byte[] _sBox;
        private static byte[] _inversedSBox;
        private static byte[,] _multiplyByModulo283Remainders;
        private static byte[] _cPolynomial;
        private static byte[] _inversedCPolynomial;
        private static byte[] _rCon;

        #endregion

        #region Fields

        private byte[] _key;
        private byte[][] _roundKeys;
        private byte _roundsCount;

        #endregion

        #region Constructors

        static AES()
        {
            MultiplyByModulo283Remainders = CreateMultiplyByModulo283Remainders();
            SBox = CreateSBox();
            InversedSBox = CreateInversedSBox(SBox);
            CPolynomial = new byte[] { 2, 3, 1, 1, 1, 2, 3, 1, 1, 1, 2, 3, 3, 1, 1, 2 };
            InversedCPolynomial = new byte[] { 14, 11, 13, 9, 9, 14, 11, 13, 13, 9, 14, 11, 11, 13, 9, 14 };
            RCon = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128, 27, 54 };
        }

        public AES()
        {
            Key = new byte[16];
        }

        #endregion

        #region Static properties

        private static byte[,] MultiplyByModulo283Remainders
        {
            get => _multiplyByModulo283Remainders ?? throw new ArgumentNullException("MultiplyByModulo283Remainders");

            set => _multiplyByModulo283Remainders = value;
        }

        private static byte[] SBox
        {
            get => _sBox ?? throw new ArgumentNullException("SBox");

            set => _sBox = value;
        }

        private static byte[] InversedSBox
        {
            get => _inversedSBox ?? throw new ArgumentNullException("InversedSBox");

            set => _inversedSBox = value;
        }

        private static byte[] CPolynomial
        {
            get => _cPolynomial ?? throw new ArgumentNullException("CPolynomial");

            set => _cPolynomial = value;
        }

        private static byte[] InversedCPolynomial
        {
            get => _inversedCPolynomial ?? throw new ArgumentNullException("InversedCPolynomial");

            set => _inversedCPolynomial = value;
        }

        private static byte[] RCon
        {
            get => _rCon ?? throw new ArgumentNullException("RCon");

            set => _rCon = value;
        }

        #endregion

        #region Properties

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

        private byte[][] RoundKeys
        {
            get => _roundKeys ?? throw new ArgumentNullException("RoundKeys");

            set => _roundKeys = value;
        }

        private byte RoundsCount
        {
            get => _roundsCount;

            set => _roundsCount = value;
        }

        #endregion

        #region Remainders, SBox, InversedSBox creation methods

        private static byte[,] CreateMultiplyByModulo283Remainders()
        {
            var result = new byte[256, 256];
            var GF256 = new GF256();

            foreach (byte i in Enumerable.Range(byte.MinValue, 256))
            {
                foreach (byte j in Enumerable.Range(i, 256 - i))
                {
                    result[i, j] = (byte)GF256.MultiplyByModulo(i, j, 283);

                    if (i != j)
                    {
                        result[j, i] = result[i, j];
                    }
                }
            }

            return result;
        }

        private static byte[] CreateSBox()
        {
            var result = new byte[256];
            var GF256 = new GF256();
            foreach (byte i in Enumerable.Range(byte.MinValue, 256))
            {
                foreach (byte j in Enumerable.Range(byte.MinValue, 256))
                {
                    if (MultiplyByModulo283Remainders[i, j] == 1)
                    {
                        Console.WriteLine($"Inverse[{i}] = {j}");
                        var MultiplicativeInversed = j;
                        for (var k = 0; k < 5; k++)
                        {
                            result[i] ^= MultiplicativeInversed;
                            Console.WriteLine($"mult inv1 :{MultiplicativeInversed}");
                            MultiplicativeInversed = (byte)((MultiplicativeInversed << 1) | (MultiplicativeInversed >> 7));
                        //    Console.WriteLine($"mult inv2 :{MultiplicativeInversed}");
                            /*if (i < 3)
                            {
                                Console.WriteLine($"k={k}");
                                Console.WriteLine($"result[{k}]={result[i]}");
                                Console.WriteLine($"inversed [{k}] {MultiplicativeInversed}");
                            }*/
                        }
                        result[i] ^=0x63;
                        break;
                    }
                }
            };
            result[0] = 0x63;
            return result;
        }

        private static byte[] CreateInversedSBox(byte[] sBox)
        {
            var result = new byte[256];
            foreach (byte i in Enumerable.Range(byte.MinValue, 256))
            {
                foreach (byte j in Enumerable.Range(byte.MinValue, 256))
                {
                    if (sBox[j] == i)
                    {
                        result[i] = j;
                        break;
                    }
                }
            }
            return result;
        }

        #endregion

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
                AddRoundKey(state, RoundKeys[round]);
            }
            SubBytes(state);
            ShiftRows(state);
            AddRoundKey(state, RoundKeys[RoundsCount - 1]);


            return state;
        }

        public byte[] Decrypt(byte[] dataBytes)
        {
            if (dataBytes.Length != 16)
            {
                throw new ArgumentException("dataBytes");
            }
            var state = (byte[])dataBytes.Clone();

            AddRoundKey(state, RoundKeys.Last());

            for (var round = 0; round < RoundsCount - 1; round++)
            {
                InversedShiftRows(state);
                InversedSubBytes(state);
                AddRoundKey(state, RoundKeys[RoundsCount - round - 2]);
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
            RoundsCount = (byte)(Key.Length / 4 + 6);
            RoundKeys = new byte[RoundsCount][];

            for (var i = 0; i < RoundsCount; i++)
            {
                RoundKeys[i] = new byte[Key.Length];
                if (i == 0)
                {
                    byte[] W = new byte[4]
                    {
                        Key[Key.Length - 3],
                        Key[Key.Length - 2],
                        Key[Key.Length - 1],
                        Key[Key.Length - 4]
                    };//циклический сдвиг


                    for (var j = 0; j < 4; j++)
                    {
                        RoundKeys[i][j] = (byte)(Key[j] ^ SBox[W[j]] ^ (j == 0 ? RCon[0] : 0));
                        Console.WriteLine($"RoundKey[{i}][{j}]"+Convert.ToString(RoundKeys[i][j],16));
                        Console.WriteLine($"i= {i} i % Rcon.Length = {i % RCon.Length} {RCon[0]}");
                    }
                        
                    for (var j = 1; j < 4; j++)
                    {
                        for (var k = 0; k < 4; k++)
                        {
                            RoundKeys[i][(j) * 4 + k] = (byte)(Key[(j) * 4 + k] ^ RoundKeys[i][(j-1) * 4 + k]);
                            Console.WriteLine($"RoundKey[{i}][{j*4+k}]" + Convert.ToString(RoundKeys[i][j], 16));
                        }
                    }
                }
                else
                {
                    var W = new byte[4]
                    {
                        RoundKeys[i - 1][Key.Length - 3],
                        RoundKeys[i - 1][Key.Length - 2],
                        RoundKeys[i - 1][Key.Length - 1],
                        RoundKeys[i - 1][Key.Length - 4]
                    };
                    for (var j = 0; j < 4; j++)
                    {
                        RoundKeys[i][j] = (byte)(RoundKeys[i - 1][j] ^ SBox[W[j]] ^ (j == 0 ? RCon[i] : 0));
                    }
                    for (var j = 1; j < 4; j++)
                    {
                        for (var k = 0; k < 4; k++)
                        {
                            RoundKeys[i][j  * 4 + k] = (byte)(RoundKeys[i - 1][j  * 4 + k] ^ RoundKeys[i][(j-1) * 4 + k]);
                        }
                    }
                }
            }
        }

        private void AddRoundKey(byte[] state, byte[] roundkey) =>
            Parallel.For(0, state.Length, (i) => state[i] ^= roundkey[i]);

        #region Secondary encryption functions

        private void SubBytes(byte[] state)
        {
            for (var i = 0; i < state.Length; i++)
            {
                state[i] = SBox[state[i]];
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
                    temporaryState[i * 4 + j] =  MultiplyByModulo283Remainders[CPolynomial[j * 4],     state[i * 4]];
                    temporaryState[i * 4 + j] ^= MultiplyByModulo283Remainders[CPolynomial[j * 4 + 1], state[i * 4 + 1]];
                    temporaryState[i * 4 + j] ^= MultiplyByModulo283Remainders[CPolynomial[j * 4 + 2], state[i * 4 + 2]];
                    temporaryState[i * 4 + j] ^= MultiplyByModulo283Remainders[CPolynomial[j * 4 + 3], state[i * 4 + 3]];
                }
            }

            Parallel.For(0, state.Length, (i) => state[i] = temporaryState[i]);
        }

        #endregion

        #region Secondary decryption functions

        private void InversedSubBytes(byte[] state)
        {
            for (var i = 0; i < state.Length; i++)
            {
                state[i] = InversedSBox[state[i]];
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
                    temporaryState[i * 4 + j] = MultiplyByModulo283Remainders[InversedCPolynomial[j * 4], state[i * 4]];
                    temporaryState[i * 4 + j] ^= MultiplyByModulo283Remainders[InversedCPolynomial[j * 4 + 1], state[i * 4 + 1]];
                    temporaryState[i * 4 + j] ^= MultiplyByModulo283Remainders[InversedCPolynomial[j * 4 + 2], state[i * 4 + 2]];
                    temporaryState[i * 4 + j] ^= MultiplyByModulo283Remainders[InversedCPolynomial[j * 4 + 3], state[i * 4 + 3]];
                }
            }
            for (var i = 0; i < state.Length; i++)
            {
                state[i] = temporaryState[i];
            }
        }

        #endregion

        #endregion
    }
}
