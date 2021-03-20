using System;
using Cryptography.Arithmetic.ResidueNumberSystem;

namespace Cryptography.DemoApplication
{
    public class PrimesNumbersJobs : BaseJobs
    {
        #region Delegates invoking tasks

        private readonly func _getPrimeNumbersLessThenMJob = () =>
        {
            int m = GetNumberFromUser();
            Console.Write($"Целые простые числа меньшие {m} : ");
            foreach (var num in ResidueNumberSystem.GetSimpleNumbersLessThenM(m)) Console.Write($"{num} ");
        };

        private readonly func _getReducedResidueSystemJob = () =>
        {
            int m = GetNumberFromUser();
            var residueNumberSystem = new ResidueNumberSystem()
            {
                Module = (ulong)m
            };
            var arr = residueNumberSystem.ReducedResidueSystem(m);
            Console.Write($"Приведеная система вычетов по модулю {m} : ");
            foreach (var num in arr) Console.Write($"{num} ");
            Console.WriteLine();
        };
        
        private readonly func _calculateEylerFunctionJob = () =>
        {
            int m = GetNumberFromUser();
            var functionValue = ResidueNumberSystem.CalculateEylerFunction((ulong)m);
            Console.WriteLine($" f({m}) = {functionValue}");
        };
        
        #endregion

        #region Utils

        private static int GetNumberFromUser()
        {
            Console.WriteLine("Введите целое число m : ");
            return Int32.Parse(Console.ReadLine());
        }

        #endregion

        public PrimesNumbersJobs()
        {
            Actions = new func[]
            {
                _getPrimeNumbersLessThenMJob,
                _getReducedResidueSystemJob,
                _calculateEylerFunctionJob
            };
        }
    }
}