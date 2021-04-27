using System;
using System.Linq;
using System.Reflection;

namespace Cryptography.DemoApplication
{
    public abstract class BaseJobs : IDemoApplicationJobs
    {
        protected delegate void func();
        protected Delegate[] Actions;
        public virtual Delegate GetJob(int jobNumber)
        {
            if (jobNumber < 1 || jobNumber > Actions.Length)
                throw new ArgumentOutOfRangeException(nameof(jobNumber),$"There is no task with number {jobNumber}");

            return Actions[jobNumber-1];
        }
        
        #region Utils

        protected static uint GetNumberFromUser(string textToUser = null, string numberType = "uint")
        {
            Console.WriteLine(textToUser ?? "Введите целое число m :");
            return ParseNeededType(Console.ReadLine(), numberType);
        }
        
        protected static uint ParseNeededType(string number, string type) =>
            type == "byte" ? byte.Parse(number) : uint.Parse(number);

        public string Help() =>
            Actions
                .Select((action, actionNumber) => $"{action.GetMethodInfo().Name} - {actionNumber + 1}")
                .Aggregate((current, next) => $"{current}\n{next}");
        

        #endregion
    }
}