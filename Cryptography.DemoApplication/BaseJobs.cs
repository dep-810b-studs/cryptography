using System;

namespace Cryptography.DemoApplication
{
    public abstract class BaseJobs : IDemoApplicationJobs
    {
        protected delegate void func();
        protected Delegate[] Actions;
        public virtual Delegate GetJob(int jobNumber)
        {
            if (jobNumber is < 1 or > 5)
                throw new ArgumentOutOfRangeException(nameof(jobNumber),$"There is no task with number {jobNumber}");

            return Actions[jobNumber-1];
        }
    }
}