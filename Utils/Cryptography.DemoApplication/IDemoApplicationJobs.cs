using System;

namespace Cryptography.DemoApplication
{
    public interface IDemoApplicationJobs
    {
        Delegate GetJob(int jobNumber);
        string Help();
    }
}