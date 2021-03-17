using System;
using System.Collections.Generic;

namespace Cryptography.DemoApplication
{
    public static class JobExecutor
    {
        public static readonly List<string> SupportedJobTypes = new() 
            {"working-with-bits", "primes-numbers"};

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
                    Console.WriteLine($"Error occured during program working: {e.InnerException.Message}");
                }

            }
        }
    }
}