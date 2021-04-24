using System;
using Cryptography.DemoApplication;

if (args.Length < 1)
{
    Console.WriteLine("You should specify tasks type to run application");
}

var jobsName = args[0];

if (JobExecutor.SupportedJobs.TryGetValue(jobsName, out var demoApplicationJobs))
{
    JobExecutor.Run(demoApplicationJobs);
}
else
{
    Console.WriteLine($"These type of jobs({jobsName}) didn't supported... ");
}




