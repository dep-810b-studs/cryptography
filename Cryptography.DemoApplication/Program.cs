using System;
using Cryptography.DemoApplication;

IDemoApplicationJobs demoApplicationJobs;

if (args.Length < 1)
{
    demoApplicationJobs = new WorkingWithBitsJobs();
}
else
{
    var jobsName = args[0];
    if (!JobExecutor.SupportedJobTypes.Contains(jobsName))
    {
        Console.WriteLine($"These type of jobs({jobsName}) didn't supported... ");
        return;
    }
    demoApplicationJobs = jobsName switch
    {
        "working-with-bits" => new WorkingWithBitsJobs(),
        "primes-numbers" => new PrimesNumbersJobs()
    };
}

JobExecutor.Run(demoApplicationJobs);

