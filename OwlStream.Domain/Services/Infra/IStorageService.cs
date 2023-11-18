using Microsoft.AspNetCore.Http;

namespace OwlStream.Domain.Services.Infra;

public interface IStorageService
{
    string BuildS3Url(string filename, string folder);
    Task<bool> Upload(IFormFile file, string filename, string folder, bool isPublic);
}