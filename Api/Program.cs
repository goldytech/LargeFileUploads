using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
// Increasing the request payload size at Kestrel level
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1024 * 1024 * 1024;
});
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapPost("/upload", async (HttpContext ctx) =>
{
    var request = ctx.Request;
    // validation of content type
    if (!request.HasFormContentType ||
        !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaType) ||
        string.IsNullOrEmpty(mediaType.Boundary.Value))
    {
        return Results.StatusCode(415);
    }
    var reader = new MultipartReader(mediaType.Boundary.Value, request.Body);
    var section = await reader.ReadNextSectionAsync();
    while (section != null)
    {
        var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);
        if (contentDisposition != null && hasContentDispositionHeader && 
            contentDisposition.DispositionType.Equals("form-data") &&
            !string.IsNullOrEmpty(contentDisposition.FileName.Value))
        {
            var fileName = Path.GetFileName(contentDisposition.FileName.Value);
            var filePath = Path.Combine(Path.GetTempPath(), fileName);
            await using var fileStream = File.Create(filePath);
            await section.Body.CopyToAsync(fileStream);
            return Results.Ok(new { filePath });
        }
        section = await reader.ReadNextSectionAsync();
    }

    return Results.BadRequest();
});
app.Run();