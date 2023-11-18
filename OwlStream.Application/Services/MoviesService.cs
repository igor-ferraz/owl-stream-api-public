using Microsoft.AspNetCore.Http;
using OwlStream.Domain.Exceptions.Services;
using OwlStream.Domain.Models.Movies;
using OwlStream.Domain.Repositories;
using OwlStream.Domain.Services.Application;
using OwlStream.Domain.Services.Infra;

namespace OwlStream.Application.Services;

public class MoviesService : IMoviesService
{
    private readonly IMoviesRepository _moviesRepository;
    private readonly IStorageService _storageService;
    private readonly IFilesValidationService _filesValidationService;

    public MoviesService(
        IMoviesRepository moviesRepository,
        IStorageService storageService,
        IFilesValidationService filesValidationService
    )
    {
        _moviesRepository = moviesRepository;
        _storageService = storageService;
        _filesValidationService = filesValidationService;
    }

    public async Task<IEnumerable<MovieResult>> Search(string text, int? genreId, string cinelistId, int limit)
    {
        if (System.String.IsNullOrEmpty(text) && (genreId is null && cinelistId is null))
        {
            return new List<MovieResult>();
        }

        if (limit == 0)
        {
            limit = 50;
        }

        return await _moviesRepository.Search(text, genreId, cinelistId, limit);
    }

    public async Task<Movie> Get(string id)
    {
        if (id is null)
        {
            throw new IncompleteModelException();
        }

        return await _moviesRepository.Get(id);
    }

    public async Task<string> Add(MovieAdd movie)
    {
        if (movie.Genres is null || !movie.Genres.Any())
        {
            throw new IncompleteModelException();
        }

        return await _moviesRepository.Add(movie);
    }

    public async Task<bool> Update(MovieUpdate movie)
    {
        return await _moviesRepository.Update(movie);
    }

    public async Task<bool> ChangeStatus(string id, bool status)
    {
        return await _moviesRepository.ChangeStatus(id, status);
    }

    public async Task<bool> Upload(string id, IFormFile file)
    {
        var validMimeTypes = new string[] { "video/mp4" };
        var valid = _filesValidationService.IsMimeType(file, validMimeTypes);

        if (!valid)
        {
            throw new InvalidFileMimeTypeException();
        }

        var folder = "movies";
        var filename = id + System.IO.Path.GetExtension(file.FileName);
        var result = await _storageService.Upload(file, filename, folder, true);

        if (result)
        {
            var url = _storageService.BuildS3Url(filename, folder);
            await _moviesRepository.AddUrl(id, "Url", url);
        }

        return result;
    }

    public async Task<bool> UploadPicture(string id, IFormFile file)
    {
        var validMimeTypes = new string[] { "image/png", "image/jpeg", "image/x-png", "image/bmp" };
        var valid = _filesValidationService.IsMimeType(file, validMimeTypes);

        if (!valid)
        {
            throw new InvalidFileMimeTypeException();
        }

        var folder = "pictures";
        var filename = id + System.IO.Path.GetExtension(file.FileName);
        var result = await _storageService.Upload(file, filename, folder, true);

        if (result)
        {
            var url = _storageService.BuildS3Url(filename, folder);
            await _moviesRepository.AddUrl(id, "PictureUrl", url);
        }

        return result;
    }
}