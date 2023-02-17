using System;
using Cryptography.Arithmetic.ResidueNumberSystem;

namespace Cryptography.DemoApplication.Jobs;

[Job("primes-numbers")]
public class PrimesNumbersJobs : BaseJobs
{
    public PrimesNumbersJobs()
    {
        Actions = new[]
        {
            _getPrimeNumbersLessThenMJob,
            _getReducedResidueSystemJob,
            _calculateEylerFunctionJob,
            _workWithFullResidueSystemJob,
            _fastPowAlgorithmJob
        };
    }

    #region Delegates invoking tasks

    /// <summary>
    ///     1. Напишите программу, выводящую все простые числа, которые меньше m.
    /// </summary>
    private readonly Job _getPrimeNumbersLessThenMJob = () =>
    {
        var m = GetNumberFromUser();
        Console.Write($"Целые простые числа меньшие {m} : ");
        var residueNumberSystem = new ResidueNumberSystem();
        var primeNumbers = residueNumberSystem.GetSimpleNumbersLessThenM((int)m);
        Console.WriteLine(string.Join(" ", primeNumbers));
    };

    /// <summary>
    ///     2. Выведите на экран приведенную систему вычетов по модулю m.
    /// </summary>
    private readonly Job _getReducedResidueSystemJob = () =>
    {
        var m = GetNumberFromUser();
        var residueNumberSystem = new ResidueNumberSystem
        {
            Module = m
        };
        var arr = residueNumberSystem.ReducedResidueSystem();
        Console.Write($"Приведеная система вычетов по модулю {m} : ");
        Console.WriteLine(string.Join(" ", arr));
    };

    /// <summary>
    ///     3. Напишите функцию, вычисляющую значение f(m), где
    ///     f(m)−функция Эйлера.
    /// </summary>
    private readonly Job _calculateEylerFunctionJob = () =>
    {
        var m = GetNumberFromUser();
        var functionValue = ResidueNumberSystem.CalculateEylerFunction((int)m);
        Console.WriteLine($" f({m}) = {functionValue}");
    };

    /// <summary>
    ///     4. Напишите библиотеку для работы с полной системой вычетов по
    ///     модулю m. Реализуйте в вашей библиотеке арифметические
    ///     операции, операцию возведения в целую степень.
    /// </summary>
    private readonly Job _workWithFullResidueSystemJob = () =>
    {
        var module = GetNumberFromUser("Введите модуль системы:");

        var residueNumberSystem = new ResidueNumberSystem
        {
            Module = module
        };

        while (true)
        {
            var firstOperand = GetNumberFromUser("Введите первый операнд:");
            var secondOperand = GetNumberFromUser("Введите второй операнд:");
            Console.WriteLine("Введите операцию, которую хотите произвести над числами(+,-,*,/,^)");
            var operation = Console.ReadLine() ?? throw new ApplicationException("Incorrect input");

            var result = operation switch
            {
                "+" => residueNumberSystem.Add(firstOperand, secondOperand),
                "-" => residueNumberSystem.Subtract(firstOperand, secondOperand),
                "*" => residueNumberSystem.Multiply(firstOperand, secondOperand),
                "/" => residueNumberSystem.Division(firstOperand, secondOperand),
                "^" => residueNumberSystem.Pow(firstOperand, secondOperand),
                _ => throw new ArgumentOutOfRangeException(nameof(operation),
                    $"This operation doesnt supported({operation})")
            };

            Console.WriteLine($"{firstOperand}{operation}{secondOperand}={result}(mod{residueNumberSystem.Module})");
            Console.WriteLine(
                $"Для продолжения работы в системе вычетов по модулю {residueNumberSystem.Module} нажмите любую клавишу. Для выхода - q.");
            var userResponse = Console.ReadLine();
            if (userResponse is not null and "q")
                return;
        }
    };

    /// <summary>
    ///     5. Реализуйте алгоритм быстрого возведения в степень в кольце вычетов.
    /// </summary>
    private readonly Job _fastPowAlgorithmJob = () =>
    {
        var m = GetNumberFromUser("Введите модуль кольца вычетов");
        var residueNumberSystem = new ResidueNumberSystem
        {
            Module = m
        };

        var number = GetNumberFromUser("Введите число, которое хотите возвести в степень");
        var degree = GetNumberFromUser("Введите степень");

        var result = residueNumberSystem.Pow(number, degree);

        Console.WriteLine($"{number}^{degree}={result}(mod {m})");
    };

    #endregion
}