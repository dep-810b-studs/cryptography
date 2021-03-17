using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cryptography.DemoApplication
{
    public class PrimesNumbersJobs : BaseJobs
    {
        #region Realizations

                        public static int[] GetSimpleNumbers(int count)
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

        public static ulong Eyler(ulong n)
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

        public static string GetDecomposition(int num)
        {
            var res = string.Empty;

            res+=$"{num} = 1";
            for (int i = 0; num % 2 == 0; num /= 2)
                res += $" * {2}";
            for (int i = 3; i <= num;)
                if (num % i == 0)
                {
                    res += $" * {i}";
                    num /= i;
                }
                else i += 2;

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

        public static long ModPowlong (long num, long degree, long modulo)
        {
            long b = 1;

            while (degree != 0)
            {
                if (degree % 2 == 0)
                {
                    degree /= 2;
                    num = (num*num)% modulo;
                }
                else
                {
                    degree--;
                    b = (b*num)%modulo;
                }
            }
            return b;
        }

        #endregion

        public PrimesNumbersJobs()
        {
            Actions = new func[]
            {
                () =>
                {
                    int m = 0;
                    do Console.Write("Введите целое число m : ");
                    while (!Int32.TryParse(Console.ReadLine(), out m));

                    Console.Write($"Целые простые числа меньшие {m} : ");

                    foreach (var num in GetSimpleNumbers(m)) Console.Write($"{num} ");

                    Console.WriteLine($"\nРазложение постепеням: {GetDecomposition(m)}");

                    Console.WriteLine($"фи({m})={Eyler((ulong) m)}");

                    var arr = ReducedResidueSystem(m);

                    Console.Write($"Приведеная система вычетов по модулю {m} : ");

                    foreach (var num in arr) Console.Write($"{num} ");

                    Console.Write("\n");
                }
            };
        }
    }
}