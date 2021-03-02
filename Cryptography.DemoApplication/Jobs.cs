using System;
using System.Linq;
using Cryptography.WorkingWithBits;

namespace Cryptography.DemoApplication
{
    public class Jobs
    {
        delegate void func();
 
        private readonly Delegate[] Actions = new func[]
        {
            ()=>
            {
                Console.WriteLine("1) Введите 32-разрядное число в двоичной сс");
                uint inputNumber = Convert.ToUInt32(Console.ReadLine(), 2);
                var text = new OpenText(inputNumber);
                Console.WriteLine("Введите номер бита, который хотите увидеть");
                int bitNumber = Convert.ToInt32(Console.ReadLine());
                var iBit = text[bitNumber];
                Console.WriteLine(iBit);
                if (iBit == 0)
                {
                    text[bitNumber] = 1;
                    Console.WriteLine($"2) Установка {bitNumber}-ого бита {Convert.ToString(text.Value,2)}");
                }

                if (iBit == 1)
                {
                    text[bitNumber] = 0;
                    Console.WriteLine($"2) Снятие {bitNumber}-ого бита {Convert.ToString(text.Value,2)}");
                }
 
                Console.WriteLine("3) Введите номер i бита для перестановки ");
                int i = Convert.ToInt32(Console.ReadLine()) ;
                Console.WriteLine("Введите номер j бита для перестановки ");
                int j = Convert.ToInt32(Console.ReadLine()) ;
                text.SwapBits(i, j);
                Console.WriteLine($"Смена местами {i}-ого и {j}-ого бит {Convert.ToString(text.Value,2)}");
                Console.WriteLine("4) Введите m бит которые хотите обнулить");
                int m = Convert.ToInt32(Console.ReadLine());
                text.ResetToZeroLowOrderBits(m);
                Console.WriteLine($"Обнуление младших {m} бит {Convert.ToString(text.Value,2)}");
            },
            ()=>
            {
                Console.WriteLine("Введите число:");
                var num = Convert.ToUInt32(Console.ReadLine(),2);
                var text = new OpenText(num);
                Console.WriteLine("Введите перестановки через пробел");
                var permutations = Console
                    .ReadLine()
                    .Split(' ')
                    .Select(byte.Parse)
                    .ToArray();
               
                text.ReplaceBitsByPermutations(permutations);
                Console.WriteLine($"Итоговое число в двоичном представлении:{Convert.ToString(text.Value,2)}");
            },
            ()=>
            {
                Console.WriteLine("Введите число:");
                var num = Convert.ToUInt32(Console.ReadLine());
                var text = new OpenText(num);
                Console.WriteLine($"Максимальная степень двойки, на которую делится число {num} = {text.FindMaxTwoDegreeThatDivisibleByNumber()}");
            },
           ()=>
           {
                Console.WriteLine("Введите число:");
                var num = Convert.ToUInt32(Console.ReadLine());
                var text = new OpenText(num);
                var p = text.GetDegreeOfTwoThatNeighborsOfNumber();
                Console.WriteLine($"Число p={p}  2^{p}<={num}<=2^{p+1}");
           },
           ()=>
           {
               //4
               Console.WriteLine("Введите число");
               var num = Convert.ToUInt32(Console.ReadLine(),2);
               var text = new OpenText(num);
               Console.WriteLine("Введите число n,на которое хотите сдвинуть:");
               var shift = Convert.ToInt32(Console.ReadLine());
               text.CyclicShift(shift, ShiftDirection.Right);
               Console.WriteLine($"Циклический сдвиг вправо на {shift} бит: {Convert.ToString(text.Value,2)}");
               text.CyclicShift(shift, ShiftDirection.Left);
               Console.WriteLine($"Циклический сдвиг влево на {shift} бит: {Convert.ToString(text.Value,2)}");
           }
        };

        public Delegate this[int index] => Actions[index];
    }
}