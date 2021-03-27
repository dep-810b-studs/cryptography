namespace Cryptography.WebInterface
{
    internal interface IMessageConvertor
    {
        ulong ConvertToLong(string message);
        string ConvertToString(ulong message);
    }
    
    internal class MessageConvertor : IMessageConvertor
    {
        public ulong ConvertToLong(string message)
        {
            return ulong.Parse(message);
        }

        public string ConvertToString(ulong message)
        {
            return message.ToString();
        }
    }
}