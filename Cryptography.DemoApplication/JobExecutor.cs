using System;
using System.Collections.Generic;
using System.Linq;
using Cryptography.DemoApplication.Jobs;

namespace Cryptography.DemoApplication
{
    public static class JobExecutor
    {
        private static readonly Dictionary<string, IDemoApplicationJobs> SupportedJobs;

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
        
        public static bool TryCreateJobs(string jobsName, out IDemoApplicationJobs demoApplicationJobs)
        {
            var selectedJobExist = SupportedJobs.TryGetValue(jobsName, out demoApplicationJobs); 
            
            if (!selectedJobExist)
                Console.WriteLine($"These type of jobs({jobsName}) didn't supported... ");
            
            return selectedJobExist;
        }
        
        public static void Run<T>(T jobs) where T : IDemoApplicationJobs
        {
            while (true)
            {
                Console.WriteLine("Please, enter task number (q to exit)");
                var userChoice = Console.ReadLine();

                if (userChoice == "q")
                    return;

                if(!Int32.TryParse(userChoice, out var jobNumber))
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
}