using System;
using Cryptography.DemoApplication;

var jobs = new Jobs();

while (true)
{
    Console.WriteLine("Please, enter task number (q to exit)");
    var userChoice = Console.ReadLine();

    if (userChoice == "q")
        return;

    if(!Int32.TryParse(userChoice, out var jobNumber) || jobNumber is < 1 or > 5)
    {
        Console.WriteLine("Entered not correct task number");
        continue;
    }
    
    var selectedJob = jobs[jobNumber-1];
    try
    {
        selectedJob.DynamicInvoke();  
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error occured during program working: {e.InnerException.Message}");
    }

}