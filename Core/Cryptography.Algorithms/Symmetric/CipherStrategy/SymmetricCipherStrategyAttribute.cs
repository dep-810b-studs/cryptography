using System;

namespace Cryptography.Algorithms.Symmetric.CipherStrategy
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class SymmetricCipherStrategyAttribute : Attribute
    {
        public SymmetricCipherStrategyAttribute(SymmetricCipherMode symmetricCipherMode)
        {
            SymmetricCipherMode = symmetricCipherMode;
        }

        public SymmetricCipherMode SymmetricCipherMode { get; init; }
    }
}