﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cryptography.Arithmetic.WorkingWithBits;

namespace Cryptography.Arithmetic.GaloisField
{
    public class GaloisField
    {
        public GaloisField(byte m)
        {
            M = m;
        }

        public byte M { get; init; }

        /// <summary>
        /// GF(256) = x^7 + x^6 + x^5 + x^4 + x^3 + x^2 + x + 1
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>

        public static string ToPotentialForm(byte number) => BinaryPolynomial.ToPotentialForm(number, 8);
    }
}