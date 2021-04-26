using Cryptography.Algorithms.Utils;
using Xunit;

namespace Cryptography.Tests
{
    public class MessageConvertorTests
    {
        [Fact]
        public void ConvertorShouldCorrectlyConvert()
        {
            //arrange
            var message = "message!";
            var convertor = new MessageConvertor();
            
            //act
            var messageInLongFormat = convertor.ConvertToLong(message);
            var messageInStringFormat = convertor.ConvertToString(messageInLongFormat);
            //assert
            Assert.Equal(message, messageInStringFormat);
        }
    }
}