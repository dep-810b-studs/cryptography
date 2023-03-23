using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Cryptography.IntegrationTests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        await using var application = new WebApplicationFactory<Program>();
        var client = application.CreateClient();

        var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello,world!"));
        using var multipartFormDataContent = new MultipartFormDataContent
        {
            { new StreamContent(stream), "inputFile", "input.txt" },
            { new StringContent("1"), "cipherAction" }
        };

        using var response = await client.PostAsync("/api/upload", multipartFormDataContent);
        await using var fileStream = await response.Content.ReadAsStreamAsync();
        using var resultStream = new MemoryStream();
        await fileStream.CopyToAsync(resultStream);
        var resultFileContent = Encoding.UTF8.GetString(resultStream.ToArray());
        Assert.Equal("Hello, world!", resultFileContent);
    }
}