using System;
using Cryptography.DemoApplication;

if (args.Length < 1)
{
    Console.WriteLine("You should specify tasks type to run application");
}

var jobsName = args[0];
var jobsCreated = JobExecutor.TryCreateJobs(jobsName, out var demoApplicationJobs);
if (jobsCreated)
{
    JobExecutor.Run(demoApplicationJobs);
}




