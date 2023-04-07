using System.Text;
using Cryptography.Algorithms.Symmetric;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Cryptography.IntegrationTests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        await using var application = new WebApplicationFactory<Program>();
        var client = application.CreateClient();

        var random = new Random();
        var randomKey = new byte[(int)CipherBlockSize.Small / 8];
        random.NextBytes(randomKey);

        var file = File.OpenRead(Path.Combine("result.txt"));
        
        using var multipartFormDataContent = new MultipartFormDataContent
        {
            { new StreamContent(file), "FormFile", "input.txt" },
            { new StreamContent(new MemoryStream(randomKey)), "KeyFile", "input.txt" },
            { new StringContent("0"), "CipherAction" },
        };

        using var response = await client.PostAsync("/api/upload", multipartFormDataContent);
        await using var fileStream = await response.Content.ReadAsStreamAsync();
        
        using var multipartFormDataContent2 = new MultipartFormDataContent
        {
            { new StreamContent(fileStream), "FormFile", "input.txt" },
            { new StreamContent(new MemoryStream(randomKey)), "KeyFile", "input.txt" },
            { new StringContent("1"), "CipherAction" },
        };
        
        using var response2 = await client.PostAsync("/api/upload", multipartFormDataContent2);
        await using var fileStream2 = await response2.Content.ReadAsStreamAsync();
        using var resultStream = new MemoryStream();
        await fileStream2.CopyToAsync(resultStream);
        var resultFileContent = Encoding.UTF8.GetString(resultStream.ToArray());

        var originalFileContent = await File.ReadAllTextAsync(Path.Combine("result.txt"));
        
        Assert.Equal(originalFileContent, resultFileContent);
    }
}