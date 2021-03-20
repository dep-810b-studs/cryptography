using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Cryptography.Arithmetic.ResidueNumberSystem
{
    public class ResidueNumberSystem
    {
        public ulong Module { set; get; }

        public ulong GreatestCommonDivisor(ulong firstNumber, ulong secondNumber)
        {
            while (secondNumber != 0)
            {
                secondNumber = firstNumber % (firstNumber = secondNumber);
            }

            return firstNumber;
        }

        public ulong CalculateEylerFunction(uint p, uint q) => (p - 1) * (q - 1);
        
        public static ulong CalculateEylerFunction(ulong N)
        {
            ulong res = N, en = Convert.ToUInt64(Math.Sqrt(N) + 1);
            for (ulong i = 2; i <= en; i++)
                if ((N % i) == 0)
                {
                    while ((N % i) == 0)
                        N /= i;
                    res -= (res / i);
                }
            if (N > 1) res -= (res / N);
            return res;
        }

        /// <summary>
        /// Solve equation a*x+b*y=gcd(a,b)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>
        /// d = gcd(a,b)
        /// </returns>
        public (ulong d, ulong x, ulong y) ExtendedEuclideanAlgorithm(ulong a, ulong b)
        {
            ulong x = 0;
            ulong y = 0;

            if (a == 0)
            {
                y = 1;
                return (b, x, y);
            }
            
            var (d,x1,y1) =  ExtendedEuclideanAlgorithm(b % a, a);

            x = y1 - (b / a) * x1;
            y = x1;
            
            return (d, x, y);
        }

        public ulong Pow(ulong number, ulong degree)
        {
            if (degree == 0)
                return 1;
            
            var z = Pow(number, degree / 2);
            var multiplier = degree % 2 == 0 ? 1 : number;
            return (multiplier * z * z) % Module;
        }

        public static int[] GetSimpleNumbersLessThenM(int count)
        {
            var numbers = new bool[count];
            var res = new List<int>();

            Parallel.For(0, count, (iterator) => numbers[iterator] = true);

            int p = 2;
            int i = 2;

            while (i * i < count)
            {
                if (numbers[i])
                    for (int j = i * i; j < count; j += i)
                        numbers[j] = false;
                i++;
            }

            int num = 0;

            foreach (var n in numbers)
            {
                if ((n) && num > 1) res.Add(num);
                num++;
            }

            return res.ToArray();
        }

        public int[] ReducedResidueSystem(int modulo)
        {
            var res = new List<int>();

            Parallel.For(0, modulo, (i) =>
            {
                if (GreatestCommonDivisor((ulong)i, (ulong)modulo) == 1) res.Add(i);
            });

            res.Sort();

            return res.ToArray();
        }
    }
}