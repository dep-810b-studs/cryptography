using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cryptography.Arithmetic.ResidueNumberSystem
{
    public class ResidueNumberSystem
    {
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
        
        public static ulong EylerFunction(ulong n)
        {
            ulong res = n, en = Convert.ToUInt64(Math.Sqrt(n) + 1);
            for (ulong i = 2; i <= en; i++)
                if ((n % i) == 0)
                {
                    while ((n % i) == 0)
                        n /= i;
                    res -= (res / i);
                }
            if (n > 1) res -= (res / n);
            return res;
        }
        
        public static int gcd(int a, int b)
        {
            while (b != 0)  b = a % (a = b);
            return a;
        }

        public static int[] ReducedResidueSystem(int modulo)
        {
            var res = new List<int>();

            Parallel.For(0, modulo, (i) =>
            {
                if (gcd(i, modulo) == 1) res.Add(i);
            });

            res.Sort();

            return res.ToArray();
        }
    }
}