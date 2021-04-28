using System;
using System.Linq;
using Cryptography.DemoApplication;

if (args.Length < 1)
{
    var possibleArguments = JobExecutor.SupportedJobs
        .Select(supportedJob => supportedJob.Key)
        .Aggregate((prev, next) => $"{prev}\n{next}");
    Console.WriteLine("You should specify tasks type to run application. Possible arguments:");
    Console.WriteLine(possibleArguments);
    return;
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




