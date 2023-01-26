using System;
using System.Linq;
using System.Text;

namespace Cryptography.Algorithms.Utils;

public interface IMessageConvertor
{
    ulong ConvertToLong(string message);
    string ConvertToString(ulong message);

    byte[] ConvertToBytes(string message, uint blockSize);
    string ConvertToString(byte[] message);
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
            for (var i = 0; i < 8 - bytesCountInMessage; i++)
                messageInBytes = messageInBytes.Prepend((byte)0).ToList();

        return messageInBytes
            .ToArray()
            .ToUInt64();
    }

    public string ConvertToString(ulong message)
    {
        var messageInBytes = message
            .ToByteArray()
            .Where(charByte => charByte != 0)
            .ToArray();

        return Encoding.UTF8.GetString(messageInBytes);
    }

    public byte[] ConvertToBytes(string message, uint blockSize)
    {
        throw new NotImplementedException();
    }

    public string ConvertToString(byte[] message)
    {
        throw new NotImplementedException();
    }
}