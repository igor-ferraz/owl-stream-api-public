using Microsoft.AspNetCore.Http;

namespace OwlStream.Domain.Services.Infra;

public interface IFilesValidationService
{
    bool IsMimeType(IFormFile file, string[] expectedMimeTypes);
}