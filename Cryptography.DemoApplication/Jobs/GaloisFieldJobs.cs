using System;
using Cryptography.Arithmetic;
using Cryptography.Arithmetic.GaloisField;

namespace Cryptography.DemoApplication.Jobs
{
    [Job("galois-field")]
    public class GaloisFieldJobs : BaseJobs
    {
        private readonly GaloisField _galoisField = new();
        
        public GaloisFieldJobs()
        {
            Actions = new Action[]
            {
                ElementGF256ToPotentialFormJob,
                MultiplicationInGF256FormJob,
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
            Console.WriteLine($"{number}=0b{Convert.ToString(number, 2)}=0x{Convert.ToString(number, 16)}={GaloisField.ToPotentialForm(number)}");
        }
        
        private void MultiplicationInGF256FormJob()
        {
            var irreduciblePolynmial = GetNumberFromUser("Умножение в GF256. Введите неприводимый полином для поля GF256:");

            _galoisField.IrreduciblePolynomial = irreduciblePolynmial;
            
            while (true)
            {
                var firstNumber = (byte)GetNumberFromUser("Введите многочлен(в десятичной СС)", "byte");
                var secondNumber = (byte)GetNumberFromUser("Введите многочлен(в десятичной СС)", "byte");

                var multiplicationResult = _galoisField.Multiply(firstNumber, secondNumber);
                
                Console.WriteLine($"{GaloisField.ToPotentialForm(firstNumber)} * {GaloisField.ToPotentialForm(secondNumber)} = {GaloisField.ToPotentialForm(multiplicationResult)}");
                Console.WriteLine($"0b{Convert.ToString(firstNumber, 2)} * 0b{Convert.ToString(secondNumber, 2)} = 0b{Convert.ToString(multiplicationResult,2)}");
                Console.WriteLine($"0x{Convert.ToString(firstNumber, 16)} * 0x{Convert.ToString(secondNumber, 2)} = 0x{Convert.ToString(multiplicationResult,2)}");
                Console.WriteLine("Для продолжения операций нажмите любую клавишу. Для выхода нажмите q");
                var userChoice = Console.ReadLine();
                
                if(userChoice == "q")
                    return;
            }
        }

        private void MultiplicationPolynomialsJob()
        {
            
        }

        private void DivisionPolynomialJob()
        {
            
        }

        private void MappingToAnotherFieldJob()
        {
            
        }
        
        private void ExtendedEclidianAlgorithmJob()
        {
            
        }

        private void FindInverseJob()
        {
            
        }

        #endregion

        #region Utils

        #endregion
    }
}