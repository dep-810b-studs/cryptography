namespace Cryptography.DemoApplication
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class JobAttribute : System.Attribute
    {
        public string Name { get; init; }

        public JobAttribute(string name)
        {
            Name = name;
        }
    }
}