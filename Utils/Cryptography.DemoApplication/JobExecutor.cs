using System;
using System.Collections.Generic;
using System.Linq;

namespace Cryptography.DemoApplication;

public static class JobExecutor
{
    static JobExecutor()
    {
        var jobsInAssemblies =
            from a in AppDomain.CurrentDomain.GetAssemblies()
            from t in a.GetTypes()
            let attributes = t.GetCustomAttributes(typeof(JobAttribute), true)
            where attributes != null && attributes.Length > 0
            select new { Type = t, JobName = attributes.Cast<JobAttribute>().First().Name };

        SupportedJobs = jobsInAssemblies
            .ToDictionary(k => k.JobName, v => Activator.CreateInstance(v.Type) as IDemoApplicationJobs);
    }

    public static Dictionary<string, IDemoApplicationJobs> SupportedJobs { get; }

    public static void Run(IDemoApplicationJobs jobs)
    {
        while (true)
        {
            Console.WriteLine("Please, enter task number (h to help, q to exit)");
            var userChoice = Console.ReadLine();

            if (userChoice == "q")
                return;

            switch (userChoice)
            {
                case "q": return;
                case "h":
                    Console.WriteLine(jobs.Help());
                    continue;
            }

            if (!int.TryParse(userChoice, out var jobNumber))
            {
                Console.WriteLine("Entered symbols cant be parsed as ints");
                continue;
            }

            try
            {
                var selectedJob = jobs.GetJob(jobNumber);
                selectedJob.DynamicInvoke();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured during program working: {e.InnerException?.Message ?? e.Message}");
            }
        }
    }
}