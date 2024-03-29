﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cryptography.Algorithms.Symmetric;
using Cryptography.Arithmetic.GaloisField;

namespace Cryptography.Algorithms.Rijandel;

internal class RijandelUtils
{
    private static readonly GaloisField _galoisField = new();

    public static readonly Dictionary<CipherBlockSize, RijandelMode> RijandelModes = new()
    {
        [CipherBlockSize.Small] = new RijandelMode(CipherBlockSize.Small, 16, 10, (1, 2, 3), 4),
        [CipherBlockSize.Middle] = new RijandelMode(CipherBlockSize.Middle, 24, 12, (1, 2, 3), 6),
        [CipherBlockSize.Big] = new RijandelMode(CipherBlockSize.Big, 32, 14, (1, 3, 4), 8)
    };

    public static IEnumerable<int> SuppotedBlockSizeCountByte =>
        RijandelModes.Select(mode => mode.Value.BlockSizeCountBytes);

    public static byte[] CreateSBox()
    {
        var result = new byte[256];

        for (var i = 1; i < 256; i++)
        {
            var MultiplicativeInversed =
                _galoisField.MultiplicativeInverse((byte)i, MultiplicativeInverseCalculationWay.Exponentiation);

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

        for (var i = 0; i < sBox.Length; i++) insersedSBox[sBox[i]] = (byte)i;

        return insersedSBox;
    }

    public static void CyclicShift(byte[] state, int shift)
    {
        if (state.Length == RijandelModes[CipherBlockSize.Big].BlockSizeCountBytes)
        {
            var ulongValue = state.ToUInt64();
            var valueForBitOperations = (ulongValue << (shift * 8)) | (ulongValue >> (state.Length - shift * 8));
            state = valueForBitOperations.ToByteArray();
        }
        else
        {
            var stateCopy = state.Clone() as byte[];

            for (var i = 0; i < state.Length; i++)
            {
                var shiftedIndex = i + shift;
                var aimIndex = shiftedIndex < state.Length ? shiftedIndex : Math.Abs(state.Length - shiftedIndex);
                state[i] = stateCopy[aimIndex];
            }
        }
    }

    public static byte[] GetRow(byte[] state, int rowNumber, int countBytesInRow)
    {
        var row = new byte[countBytesInRow];

        for (var i = 0; i < countBytesInRow; i++) row[i] = state[i + rowNumber * countBytesInRow];

        return row;
    }

    public static void SetRow(byte[] state, byte[] row, int rowNumber, int countBytesInRow)
    {
        for (var i = 0; i < countBytesInRow; i++) state[i + rowNumber * countBytesInRow] = row[i];
    }
}