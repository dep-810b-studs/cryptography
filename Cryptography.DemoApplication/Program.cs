using System;
using Cryptography.DemoApplication;

var jobs = new Jobs();
Console.WriteLine("Выберите задачу");
int numberTask = Convert.ToInt32(Console.ReadLine());
var selectedJob = jobs[numberTask-1];
selectedJob.DynamicInvoke();  