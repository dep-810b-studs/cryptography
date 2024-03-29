﻿using System.Collections.Generic;
using System.Linq;

namespace Cryptography.Arithmetic.ResidueNumberSystem;

public interface IResidueNumberSystem
{
    public ulong Module { set; get; }
    ulong CalculateEylerFunction(uint p, uint q);
    ulong MultiplicativeInverse(ulong number, ulong eylerFunctionValue);
    ulong Pow(ulong number, ulong degree);
    ulong GreatestCommonDivisor(ulong firstNumber, ulong secondNumber);
    IEnumerable<int> GetSimpleNumbersLessThenM(int count);
}

public class ResidueNumberSystem : IResidueNumberSystem
{
    public ulong Module { get; set; }

    public ulong GreatestCommonDivisor(ulong firstNumber, ulong secondNumber)
    {
        while (secondNumber != 0) secondNumber = firstNumber % (firstNumber = secondNumber);

        return firstNumber;
    }

    public ulong CalculateEylerFunction(uint p, uint q)
    {
        return (p - 1) * (q - 1);
    }

    public IEnumerable<int> GetSimpleNumbersLessThenM(int count)
    {
        var numberIsPrimeMapping = Enumerable
            .Repeat(true, count - 1)
            .Prepend(false)
            .Select((isPrime, index) => (index, isPrime))
            .ToArray();

        var p = 2;

        while (p * p < count)
        {
            for (var i = p * p; i < count; i += p) numberIsPrimeMapping[i].isPrime = false;

            p = numberIsPrimeMapping
                .First(number => number.index > p && number.isPrime)
                .index;
        }

        return numberIsPrimeMapping
            .Where(tuple => tuple.isPrime)
            .Select(tuple => tuple.index);
    }

    public static int CalculateEylerFunction(int N)
    {
        var result = N;

        for (var i = 2; i * i <= N; i++)
        {
            if (N % i == 0)
            {
                while (N % i == 0) N /= i;

                result -= result / i;
            }

            if (N > 1)
                result -= result / N;
        }

        return result;
    }

    /// <summary>
    ///     Solve equation a*x+b*y=gcd(a,b)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>
    ///     d = gcd(a,b)
    /// </returns>
    public (long d, long x, long y) ExtendedEuclideanAlgorithm(long a, long b)
    {
        if (a == 0) return (b, 0, 1);

        var (d, x1, y1) = ExtendedEuclideanAlgorithm(b % a, a);
        var x = y1 - b / a * x1;
        return (d, x, x1);
    }

    public IEnumerable<int> ReducedResidueSystem()
    {
        return Enumerable
            .Range(1, (int)Module)
            .Where(i => GreatestCommonDivisor((ulong)i, Module) == 1);
    }

    #region AriphmeticOperations

    public ulong Add(ulong firstNumber, ulong secondNumber)
    {
        return (firstNumber + secondNumber) % Module;
    }

    public ulong Subtract(ulong firstNumber, ulong secondNumber)
    {
        return (firstNumber - secondNumber) % Module;
    }

    public ulong Multiply(ulong firstNumber, ulong secondNumber)
    {
        return firstNumber * secondNumber % Module;
    }

    /// <remarks>
    ///     Поддерживается деление только по простому модулю
    ///     a / b (mod M) = a * b^-1 (mod M) = a * b^(M-2) (mod M)
    ///     основано на  a^(M-2)=a^(-1) (mod M)
    /// </remarks>
    public ulong Division(ulong firstNumber, ulong secondNumber)
    {
        return Multiply(firstNumber, Pow(secondNumber, Module - 2));
    }


    public ulong MultiplicativeInverse(ulong number, ulong eylerFunctionValue)
    {
        var (_, x, _) = ExtendedEuclideanAlgorithm((long)number, (long)eylerFunctionValue);
        var inverseNumber = 2 * (long)eylerFunctionValue + x % (long)eylerFunctionValue;
        return (ulong)inverseNumber;
    }

    public ulong Pow(ulong number, ulong degree)
    {
        ulong result = 1;

        while (degree != 0)
        {
            if ((degree & 1) == 1) result = result * number % Module;

            number = number * number % Module;
            degree >>= 1;
        }

        return result;
    }

    #endregion
}