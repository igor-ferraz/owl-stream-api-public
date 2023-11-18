using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OwlStream.Domain.Services.Infra;

namespace OwlStream.Infra.Services;

public class StorageService : IStorageService
{
    private static IConfiguration _configuration;

    public StorageService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string BuildS3Url(string filename, string folder)
    {
        var bucket = _configuration.GetSection("S3Config:bucket").Value;
        var url = _configuration.GetSection("S3Config:url").Value;

        return url.Replace("{bucket}", bucket).Replace("{key}", folder).Replace("{filename}", filename);
    }

    public async Task<bool> Upload(IFormFile file, string filename, string folder, bool isPublic)
    {
        try
        {
            var bucket = _configuration.GetSection("S3Config:bucket").Value;
            var accessKey = _configuration.GetSection("S3Config:accessKey").Value;
            var secretKey = _configuration.GetSection("S3Config:secretKey").Value;
            var region = RegionEndpoint.GetBySystemName(_configuration.GetSection("S3Config:region").Value);

            var config = new AmazonS3Config
            {
                RegionEndpoint = region,
                ServiceURL = $"https://s3.amazonaws.com"
            };

            using (IAmazonS3 s3Client = new AmazonS3Client(new BasicAWSCredentials(accessKey, secretKey), config))
            {
                var request = new PutObjectRequest
                {
                    InputStream = file.OpenReadStream(),
                    BucketName = bucket,
                    Key = $"{folder}/{filename}",
                    ContentType = file.ContentType,
                    CannedACL = isPublic ? S3CannedACL.PublicRead : S3CannedACL.Private
                };

                var response = await s3Client.PutObjectAsync(request);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}