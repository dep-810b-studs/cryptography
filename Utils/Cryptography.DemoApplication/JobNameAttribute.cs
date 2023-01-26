using System;

namespace Cryptography.DemoApplication;

[AttributeUsage(AttributeTargets.Class)]
public class JobAttribute : Attribute
{
    public JobAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; init; }
}