using System;
using System.Linq;
using System.Text;
using Cryptography.Algorithms;

namespace Cryptography.WebInterface
{
    public interface IMessageConvertor
    {
        ulong ConvertToLong(string message);
        string ConvertToString(ulong message);
    }
    
    public class MessageConvertor : IMessageConvertor
    {
        private const uint _maxMessageSizeInBytes = 8;
        
        public ulong ConvertToLong(string message)
        {
            var bytesCountInMessage = Encoding.UTF8.GetByteCount(message); 
            
            if (Encoding.UTF8.GetByteCount(message) > _maxMessageSizeInBytes)
                throw new ArgumentException("Message have too large size");

            var messageInBytes = Encoding.UTF8.GetBytes(message).ToList(); 
            
            if (bytesCountInMessage < 8)
            {
                for (int i = 0; i < 8 - bytesCountInMessage; i++)
                {
                    messageInBytes = messageInBytes.Prepend((byte)0).ToList();
                }
            }
            
            return messageInBytes
                .ToArray()
                .ToUInt64();

            //return ulong.Parse(message);
        }

        public string ConvertToString(ulong message)
        {
            var messageInBytes = message
                .ToByteArray()
                .Where(charByte => charByte != 0)
                .ToArray();
            
            return Encoding.UTF8.GetString(messageInBytes);

            //return message.ToString();
        }
    }
}