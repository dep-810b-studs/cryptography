using Cryptography.Algorithms.Symmetric;
using Cryptography.Algorithms.Symmetric.CipherSystem;
using Microsoft.AspNetCore.Mvc;

namespace Cryptography.API.Controllers;

[Route("api")]
public class UploadController : Controller
{
    private readonly ISymmetricSystem _symmetricSystem;

    public UploadController(ISymmetricSystem symmetricSystem)
    {
        _symmetricSystem = symmetricSystem;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm]FileRequest inputFile,
        [FromForm] CipherAction cipherAction)
    {
        using var fileStream = new MemoryStream();
        using var keyStream = new MemoryStream();
        
        await inputFile.FormFile.CopyToAsync(fileStream);
        await inputFile.KeyFile.CopyToAsync(keyStream);

        var result = _symmetricSystem.HandleEncryption(cipherAction,
            SymmetricCipherMode.ElectronicCodeBook,
            CipherBlockSize.Small,
            fileStream.ToArray(),
            keyStream.ToArray());
        
        return new FileContentResult(result,"text/txt") { FileDownloadName = "result.txt" };
    }

    [HttpPost("keys/random")]
    public IActionResult GenerateRandom()
    {
        var key = _symmetricSystem.GenerateRandomKey();
        return new FileContentResult(key,"text/txt") { FileDownloadName = "key" };
    }
}