using System;
using System.Collections.Generic;

namespace Cryptography.DemoApplication
{
    public static class JobExecutor
    {
        private static readonly List<string> SupportedJobTypes = new() 
            {"working-with-bits", "primes-numbers"};

        public static bool TryCreateJobs(string jobsName, out IDemoApplicationJobs demoApplicationJobs)
        {
            demoApplicationJobs = null;
                
            if (!SupportedJobTypes.Contains(jobsName))
            {
                Console.WriteLine($"These type of jobs({jobsName}) didn't supported... ");
                return false;
            }
            
            demoApplicationJobs = jobsName switch
            {
                "working-with-bits" => new WorkingWithBitsJobs(),
                "primes-numbers" => new PrimesNumbersJobs()
            };

            return true;
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