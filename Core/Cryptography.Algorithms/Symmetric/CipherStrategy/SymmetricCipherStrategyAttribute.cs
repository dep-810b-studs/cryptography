using System;

namespace Cryptography.Algorithms.Symmetric.CipherStrategy;

[AttributeUsage(AttributeTargets.Class)]
public class SymmetricCipherStrategyAttribute : Attribute
{
    public SymmetricCipherStrategyAttribute(SymmetricCipherMode symmetricCipherMode)
    {
        SymmetricCipherMode = symmetricCipherMode;
    }

    public SymmetricCipherMode SymmetricCipherMode { get; init; }
}