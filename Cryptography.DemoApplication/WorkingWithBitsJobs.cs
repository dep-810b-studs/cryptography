using System;
using System.Linq;
using Cryptography.Arithmetic.WorkingWithBits;

namespace Cryptography.DemoApplication
{
    public class WorkingWithBitsJobs : BaseJobs
    {
        #region Delegates invoking tasks
        
        ///<summary>
        /// 1st task
        /// 1. Вывести k-ый бит числа a. Номер бита предварительно запросить у пользователя.
        ///  2. Установить/снять k-ый бит числа a.
        /// 3. Поменять местами i-ый и j-ый биты в числе a. Числа i и j предварительно запросить у пользователя.
        /// 4. Обнулить младшие m бит
        /// </summary>
        private readonly func _manipulatingBits = () =>
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
                Console.WriteLine($"2) Установка {bitNumber}-ого бита {Convert.ToString(text.Value, 2)}");
            }

            if (iBit == 1)
            {
                text[bitNumber] = 0;
                Console.WriteLine($"2) Снятие {bitNumber}-ого бита {Convert.ToString(text.Value, 2)}");
            }

            Console.WriteLine("3) Введите номер i бита для перестановки ");
            int i = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Введите номер j бита для перестановки ");
            int j = Convert.ToInt32(Console.ReadLine());
            text.SwapBits(i, j);
            Console.WriteLine($"Смена местами {i}-ого и {j}-ого бит {Convert.ToString(text.Value, 2)}");
            Console.WriteLine("4) Введите m бит которые хотите обнулить");
            int m = Convert.ToInt32(Console.ReadLine());
            text.ResetToZeroLowOrderBits(m);
            Console.WriteLine($"Обнуление младших {m} бит {Convert.ToString(text.Value, 2)}");
        };

        ///<summary>
        /// 2nd task
        /// 5. Поменять местами байты в заданном 32-х разрядном целом числе. Перестановка задается пользователем.
        /// </summary>
        private readonly func _swappingBytes = () =>
        {
            Console.WriteLine("Please, enter a number in a binary representation:");
            var num = Convert.ToUInt32(Console.ReadLine(), 2);
            var text = new OpenText(num);
            Console.WriteLine("Please enter permutations array separated by space");
            var permutations = Console
                .ReadLine()
                .Split(' ')
                .Select(byte.Parse)
                .ToArray();

            text.ReplaceBytesByPermutations(permutations);
            Console.WriteLine($"Result in a binary representation:{Convert.ToString(text.Value, 2)}");
        };

        ///<summary>
        /// 3rd task
        /// 6. Найти максимальную степень 2, на которую делится данное целое число. Примечание. Операторами цикла пользоваться нельзя.
        /// </summary>
        private readonly func _maxTwoDegree = () =>
        {
            Console.WriteLine("Введите число:");
            var num = Convert.ToUInt32(Console.ReadLine());
            var text = new OpenText(num);
            Console.WriteLine(
                $"Максимальная степень двойки, на которую делится число {num} = {text.FindMaxTwoDegreeThatDivisibleByNumber()}");
        };

        ///<summary>
        /// 4th task
        /// 7. Пусть x целое число. Найти такое p, что 2^p<=x<=2^(p+1).
        /// </summary>
        private readonly func _numberBetween = () =>
        {
            Console.WriteLine("Введите число:");
            var num = Convert.ToUInt32(Console.ReadLine());
            var text = new OpenText(num);
            var p = text.GetDegreeOfTwoThatNeighborsOfNumber();
            Console.WriteLine($"Число p={p}  2^{p}<={num}<=2^{p + 1}");
        };

        ///<summary>
        /// 5th task
        /// 8. Написать методы циклического сдвига в 2^p разрядном целом числе на n бит влево и вправо
        /// </summary>
        private readonly func _cyclicShift = () =>
        {
            Console.WriteLine("Введите число");
            var num = Convert.ToUInt32(Console.ReadLine(), 2);
            var textForRightShift = new OpenText(num);
            var textForLeftShift = new OpenText(num);
            Console.WriteLine("Введите число n,на которое хотите сдвинуть:");
            var shift = Convert.ToInt32(Console.ReadLine());
            textForRightShift.CyclicShift(shift, ShiftDirection.Right);
            Console.WriteLine(
                $"Циклический сдвиг вправо на {shift} бит: {Convert.ToString(textForRightShift.Value, 2)}");
            textForLeftShift.CyclicShift(shift, ShiftDirection.Left);
            Console.WriteLine(
                $"Циклический сдвиг влево на {shift} бит: {Convert.ToString(textForLeftShift.Value, 2)}");
        }; 
        
        #endregion
        public WorkingWithBitsJobs()
        {
            Actions = new []
                {
                    _manipulatingBits,
                    _swappingBytes,
                    _maxTwoDegree,
                    _numberBetween,
                    _cyclicShift
                };
        }
    }
}