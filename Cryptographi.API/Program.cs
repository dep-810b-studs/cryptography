using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//todo: use controller for form data input
app.MapPost("/api/upload", async (HttpContext context, CancellationToken cancellationToken) =>
{
    var reader = new MultipartReader("input", context.Request.Body);
    var content = await reader.ReadNextSectionAsync(cancellationToken);
    var stream = content?.AsFileSection()?.FileStream;
    return stream ?? Stream.Null;
});

app.MapPost("/api/upload2", async ([FromForm] FileRequest inputFile, CancellationToken cancellationToken) =>
{
    var memoryStream = new MemoryStream();
    await inputFile.FormFile.CopyToAsync(memoryStream);
    return new FileStreamResult(memoryStream, "text/txt") { FileDownloadName = "result.txt" };
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

public partial class Program
{
}

public class FileRequest
{
    public IFormFile FormFile { get; set; }
}