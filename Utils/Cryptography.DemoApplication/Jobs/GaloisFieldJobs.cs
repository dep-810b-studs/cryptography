using System;
using System.Diagnostics;
using Cryptography.Arithmetic;
using Cryptography.Arithmetic.GaloisField;

namespace Cryptography.DemoApplication.Jobs;

[Job("galois-field")]
public class GaloisFieldJobs : BaseJobs
{
    private readonly GaloisField _galoisField = new();

    public GaloisFieldJobs()
    {
        Actions = new Delegate[]
        {
            ElementGF256ToPotentialFormJob,
            MultiplicationInGF256Job,
            MultiplicationPolynomialsJob,
            DivisionPolynomialJob,
            ExtendedEclidianAlgorithmJob,
            FindInverseJob,
            MappingToAnotherFieldJob
        };
    }

    #region Delegates invoking tasks

    private void ElementGF256ToPotentialFormJob()
    {
        var number = (byte)GetNumberFromUser("Введите элемент GF256, который хотите представить в виде полинома");
        Console.WriteLine(
            $"{number}=0b{Convert.ToString(number, 2)}=0x{Convert.ToString(number, 16)}={GaloisField.ToPotentialForm(number)}");
    }

    private void MultiplicationInGF256Job()
    {
        var irreduciblePolynmial = GetNumberFromUser("Умножение в GF256. Введите неприводимый полином для поля GF256:");

        _galoisField.IrreduciblePolynomial = irreduciblePolynmial;

        while (true)
        {
            var firstNumber = (byte)GetNumberFromUser("Введите многочлен(в десятичной СС)", "byte");
            var secondNumber = (byte)GetNumberFromUser("Введите многочлен(в десятичной СС)", "byte");

            var multiplicationResult = _galoisField.Multiply(firstNumber, secondNumber);

            Console.WriteLine($"{firstNumber} * {secondNumber} = {multiplicationResult}");
            Console.WriteLine(
                $"({GaloisField.ToPotentialForm(firstNumber)}) * ({GaloisField.ToPotentialForm(secondNumber)}) = {GaloisField.ToPotentialForm(multiplicationResult)}");
            Console.WriteLine(
                $"0b{Convert.ToString(firstNumber, 2)} * 0b{Convert.ToString(secondNumber, 2)} = 0b{Convert.ToString(multiplicationResult, 2)}");
            Console.WriteLine(
                $"0x{Convert.ToString(firstNumber, 16)} * 0x{Convert.ToString(secondNumber, 16)} = 0x{Convert.ToString(multiplicationResult, 16)}");
            Console.WriteLine("Для продолжения операций нажмите любую клавишу. Для выхода нажмите q");
            var userChoice = Console.ReadLine();

            if (userChoice == "q")
                return;
        }
    }

    private void MultiplicationPolynomialsJob()
    {
        while (true)
        {
            BinaryPolynomial firstNumber = GetNumberFromUser("Введите многочлен(в десятичной СС)");
            BinaryPolynomial secondNumber = GetNumberFromUser("Введите многочлен(в десятичной СС)");

            var multiplicationResult = firstNumber * secondNumber;

            Console.WriteLine($"{firstNumber.Value} * {secondNumber.Value} = {multiplicationResult.Value}");
            Console.WriteLine($"({firstNumber}) * ({secondNumber}) = ({multiplicationResult})");
            Console.WriteLine(
                $"0b{Convert.ToString(firstNumber, 2)} * 0b{Convert.ToString(secondNumber, 2)} = 0b{Convert.ToString(multiplicationResult, 2)}");
            Console.WriteLine(
                $"0x{Convert.ToString(firstNumber, 16)} * 0x{Convert.ToString(secondNumber, 16)} = 0x{Convert.ToString(multiplicationResult, 16)}");
            Console.WriteLine("Для продолжения операций нажмите любую клавишу. Для выхода нажмите q");
            var userChoice = Console.ReadLine();

            if (userChoice == "q")
                return;
        }
    }

    private void DivisionPolynomialJob()
    {
        while (true)
        {
            BinaryPolynomial firstNumber = GetNumberFromUser("Введите многочлен(в десятичной СС)");
            BinaryPolynomial secondNumber = GetNumberFromUser("Введите многочлен(в десятичной СС)");

            var multiplicationResult = firstNumber % secondNumber;

            Console.WriteLine($"{firstNumber.Value} % {secondNumber.Value} = {multiplicationResult.Value}");
            Console.WriteLine($"({firstNumber}) % ({secondNumber}) = ({multiplicationResult})");
            Console.WriteLine(
                $"0b{Convert.ToString(firstNumber, 2)} % 0b{Convert.ToString(secondNumber, 2)} = 0b{Convert.ToString(multiplicationResult, 2)}");
            Console.WriteLine(
                $"0x{Convert.ToString(firstNumber, 16)} % 0x{Convert.ToString(secondNumber, 16)} = 0x{Convert.ToString(multiplicationResult, 16)}");
            Console.WriteLine("Для продолжения операций нажмите любую клавишу. Для выхода нажмите q");
            var userChoice = Console.ReadLine();

            if (userChoice == "q")
                return;
        }
    }


    private void ExtendedEclidianAlgorithmJob()
    {
        while (true)
        {
            BinaryPolynomial firstNumber = GetNumberFromUser("Введите многочлен(в десятичной СС)");
            BinaryPolynomial secondNumber = GetNumberFromUser("Введите многочлен(в десятичной СС)");

            var (gcd, x, y) = BinaryPolynomial.ExtendedEuclideanAlgorithm(firstNumber, secondNumber);

            Console.WriteLine($"{firstNumber.Value} * {x.Value} + {secondNumber.Value} * {y.Value} = {gcd.Value}");
            Console.WriteLine($"({firstNumber}) * ({x}) + ({secondNumber}) * ({y}) = {gcd}");
            Console.WriteLine(
                $"0b{Convert.ToString(firstNumber.Value, 2)} *  0b{Convert.ToString(x.Value, 2)} + 0b{Convert.ToString(secondNumber.Value, 2)} * 0b{Convert.ToString(y.Value, 2)} = 0b{Convert.ToString(gcd.Value, 2)}");
            Console.WriteLine(
                $"0x{Convert.ToString(firstNumber.Value, 16)} *  0x{Convert.ToString(x.Value, 16)} + 0x{Convert.ToString(secondNumber.Value, 16)} * 0x{Convert.ToString(y.Value, 16)} = 0x{Convert.ToString(gcd.Value, 16)}");
            Console.WriteLine("Для продолжения операций нажмите любую клавишу. Для выхода нажмите q");
            var userChoice = Console.ReadLine();

            if (userChoice == "q")
                return;
        }
    }

    private void FindInverseJob()
    {
        while (true)
        {
            var irreduciblePolynmial =
                GetNumberFromUser("Поиск обратного в GF256. Введите неприводимый полином для поля GF256:");
            var number = (byte)GetNumberFromUser("Введите многочлен(в десятичной СС)", "byte");

            _galoisField.IrreduciblePolynomial = irreduciblePolynmial;

            var methodExecutionWatch = Stopwatch.StartNew();
            var inverseUsingPotential =
                _galoisField.MultiplicativeInverse(number, MultiplicativeInverseCalculationWay.Exponentiation);
            methodExecutionWatch.Stop();
            Console.WriteLine(
                $"Exponentiation. {number}^-1 = {inverseUsingPotential}. Time: {methodExecutionWatch.ElapsedMilliseconds} ms");

            methodExecutionWatch.Restart();
            var inverseUsingGcdExt = _galoisField.MultiplicativeInverse(number,
                MultiplicativeInverseCalculationWay.ExtendedEuclideanAlgorithm);
            methodExecutionWatch.Stop();
            Console.WriteLine(
                $"ExtendedEuclideanAlgorithm. {number}^-1 = {inverseUsingGcdExt}. Time: {methodExecutionWatch.ElapsedMilliseconds} ms");

            Console.WriteLine("Для продолжения операций нажмите любую клавишу. Для выхода нажмите q");
            var userChoice = Console.ReadLine();

            if (userChoice == "q")
                return;
        }
    }

    private void MappingToAnotherFieldJob()
    {
        while (true)
        {
            var fromIrreduciblePolynmial =
                GetNumberFromUser(
                    "Отображение между полями Галуа. Введите неприводимый полином для исходного поля GF256:");

            var number = (byte)GetNumberFromUser("Введите многочлен(в десятичной СС)", "byte");
            var toIrreduciblePolynomial =
                GetNumberFromUser("Введите неприводимый полином поля, в которое нужно установить соответствие:");

            _galoisField.IrreduciblePolynomial = fromIrreduciblePolynmial;
            var mappingResult = _galoisField.ToAnotherField(number, toIrreduciblePolynomial);

            Console.WriteLine($"{number} -> {mappingResult}");
            Console.WriteLine($"{GaloisField.ToPotentialForm(number)} -> {GaloisField.ToPotentialForm(mappingResult)}");
            Console.WriteLine($"0b{Convert.ToString(number, 2)} -> 0b{Convert.ToString(mappingResult, 2)}");
            Console.WriteLine($"0x{Convert.ToString(number, 16)} -> 0x{Convert.ToString(mappingResult, 16)}");
            Console.WriteLine("Для продолжения операций нажмите любую клавишу. Для выхода нажмите q");
            var userChoice = Console.ReadLine();

            if (userChoice == "q")
                return;
        }
    }

    #endregion
}