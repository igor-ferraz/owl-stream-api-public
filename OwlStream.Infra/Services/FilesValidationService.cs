using Microsoft.AspNetCore.Http;
using FileSignatures;
using OwlStream.Domain.Services.Infra;

namespace OwlStream.Infra.Services;

public class FilesValidationService : IFilesValidationService
{
    private readonly IFileFormatInspector inspector;

    public FilesValidationService()
    {
        inspector = new FileFormatInspector();
    }

    public bool IsMimeType(IFormFile file, string[] expectedMimeTypes)
    {
        var mimeType = inspector.DetermineFileFormat(file.OpenReadStream());
        return expectedMimeTypes.Contains(mimeType.ToString());
    }
}