﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cryptography.Algorithms.Symmetric;

namespace Cryptography.Algorithms.DES
{
    public class DesCipher : ISymmetricCipher
    {
        #region Tables

                static int[] pc1 = {
            57, 49, 41, 33, 25, 17, 9,
            1, 58, 50, 42, 34, 26, 18,
            10, 2, 59, 51, 43, 35, 27,
            19, 11, 3, 60, 52, 44, 36,
            63, 55, 47, 39, 31, 23, 15,
            7, 62, 54, 46, 38, 30, 22,
            14, 6, 61, 53, 45, 37, 29,
            21, 13, 5, 28, 20, 12, 4
                            };
        static int[] shifts = {
            1, 1, 2, 2, 2, 2, 2, 2,
            1, 2, 2, 2, 2, 2, 2, 1
                            };
        static int[] pc2 = {
            14, 17, 11, 24, 1, 5,
            3, 28, 15, 6, 21, 10,
            23, 19, 12, 4, 26, 8,
            16, 7, 27, 20, 13, 2,
            41, 52, 31, 37, 47, 55,
            30, 40, 51, 45, 33, 48,
            44, 49, 39, 56, 34, 53,
            46, 42, 50, 36, 29, 32
                            };
        static int[] ip = {
            58, 50, 42, 34, 26, 18, 10, 2,
            60, 52, 44, 36, 28, 20, 12, 4,
            62, 54, 46, 38, 30, 22, 14, 6,
            64, 56, 48, 40, 32, 24, 16, 8,
            57, 49, 41, 33, 25, 17, 9, 1,
            59, 51, 43, 35, 27, 19, 11, 3,
            61, 53, 45, 37, 29, 21, 13, 5,
            63, 55, 47, 39, 31, 23, 15, 7
                            };
        static int[] exp = {
            32, 1, 2, 3, 4, 5,
            4, 5, 6, 7, 8, 9,
            8, 9, 10, 11, 12, 13,
            12, 13, 14, 15, 16, 17,
            16, 17, 18, 19, 20, 21,
            20, 21, 22, 23, 24, 25,
            24, 25, 26, 27, 28, 29,
            28, 29, 30, 31, 32, 1
                            };
        static int[][][] sbox = {
                   new int[][]{
                        new int[]{14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7 },
                        new int[]{0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8 },
                        new int[]{4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0 },
                        new int[]{15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13 }
                           },
                    new int[][]{
                        new int[]{15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10 },
                        new int[]{3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5 },
                        new int[]{0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15 },
                        new int[]{13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9 }
                             },
                    new int[][]{
                    new int[]{10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8 },
                    new int[]{13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1 },
                    new int[]{13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7 },
                    new int[]{1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12}
                    },
                    new int[][]{
                    new int[]{ 7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15 },
                    new int[]{ 13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9 },
                    new int[]{ 10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4 },
                    new int[]{ 3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14 }
                    },

                    new int[][]{
                    new int[]{ 2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9 },
                    new int[]{ 14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6 },
                    new int[]{ 4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14},
                    new int[]{ 11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3}
                    },
                    new int[][]{
                    new int[]{ 12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11 },
                    new int[]{  10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8 },
                    new int[]{ 9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6 },
                    new int[]{ 4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13 }
                    },
                    new int[][]{
                    new int[]{ 4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1 },
                    new int[]{ 13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6 },
                    new int[]{ 1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2 },
                    new int[]{ 6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12 }
                    },
                    new int[][]{
                    new int[]{ 13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7 },
                    new int[]{ 1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2 },
                    new int[]{ 7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8 },
                    new int[]{ 2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11 }
                    }
                    };
        static int[] p = {  16, 7, 20, 21,
                            29, 12, 28, 17,
                            1, 15, 23, 26,
                            5, 18, 31, 10,
                            2, 8, 24, 14,
                            32, 27, 3, 9,
                            19, 13, 30, 6,
                            22, 11, 4, 25};
        static int[] ip1 = {40, 8, 48, 16, 56, 24, 64, 32,
                            39, 7, 47, 15, 55, 23, 63, 31,
                            38, 6, 46, 14, 54, 22, 62, 30,
                            37, 5, 45, 13, 53, 21, 61, 29,
                            36, 4, 44, 12, 52, 20, 60, 28,
                            35, 3, 43, 11, 51, 19, 59, 27,
                            34, 2, 42, 10, 50, 18, 58, 26,
                            33, 1, 41, 9, 49, 17, 57, 25};

        #endregion

        private static BitArray EncryptionConvertion(BitArray message, BitArray[] keys, CipherAction cipherAction)
        {
            message.applyPermutations(ip);

            if (cipherAction is CipherAction.Decrypt)
            {
                var temp = new List<BitArray>(keys);
                temp.Reverse();
                keys = temp.ToArray();   
            }

            var twoParts = message.DevideByParts(2);
            var L = twoParts[0];
            var R = twoParts[1];

            for (int i = 0; i < 16;i++)
            {
                var tempL = L.Clone() as BitArray;
                L = R.Clone() as BitArray;
                R = tempL ^ F(R, keys[i]);
            }

            var res = R.JoinArr(L);

            res.applyPermutations(ip1);

            return res;
        }
        
        private static BitArray F(BitArray right, BitArray key)
        {
            var extendedRight = new BitArray(48);

            for (int i = 0; i < 48; ++i) extendedRight[i] = right[exp[i]-1];

            var rightXorArr = extendedRight.Clone() as BitArray ^ key;

            var B = rightXorArr.DevideByParts(8);


            for(int k =0; k < 8;++k)
            {
                int i = ((B[k][0] ? 1 : 0) << 1) | (B[k][5] ? 1 : 0);
                int j = ((B[k][4] ? 1 : 0)) | ((B[k][3] ? 1 : 0) << 1) | ((B[k][2] ? 1 : 0) << 2) | ((B[k][1] ? 1 : 0) << 3);
                B[k] = new BitArray(Convert.ToString(sbox[k][i][j], 2), 4);
            }

            var S = BitArray.ConcatArr(B);

            S.applyPermutations(p);

            return S;
        }
        private static BitArray[] Key(BitArray keyInBitArr)
        {
            if (keyInBitArr.Count < 64) keyInBitArr.IncreaseLength(64-keyInBitArr.Count);

            keyInBitArr.applyPermutations(pc1);
            var twoParts = keyInBitArr.DevideByParts(2);

            var C = new BitArray[16];
            var D = new BitArray[16];

            for(int i =0; i <16;++i)
            {
                C[i] = twoParts[0] << shifts[i];
                D[i] = twoParts[1] << shifts[i];
            }

            var CD = new List<BitArray>();

            for (int i = 0; i < 16; ++i) CD.Add(C[i].JoinArr(D[i]));

            var keys = CD;

            foreach (var k in keys) k.applyPermutations(pc2);

            return keys.ToArray();
        }

        public CipherBlockSize CipherBlockSize { get; set; } = CipherBlockSize.Des;

        public byte[] Encrypt(byte[] openText, byte[] key)
        {
            var textInBitArrayFormat = new BitArray(openText);
            var keyInBitArrayFormat = Key(new BitArray(key));
            var cipherText =  EncryptionConvertion(textInBitArrayFormat, keyInBitArrayFormat, CipherAction.Encrypt);
            return cipherText.getByteArrayFromBitArray;
        }

        public byte[] Decrypt(byte[] cipherText, byte[] key)
        {
            var textInBitArrayFormat = new BitArray(cipherText);
            var keyInBitArrayFormat = Key(new BitArray(key));
            var openText =  EncryptionConvertion(textInBitArrayFormat, keyInBitArrayFormat, CipherAction.Decrypt);
            return openText.getByteArrayFromBitArray;
        }
    }
}
