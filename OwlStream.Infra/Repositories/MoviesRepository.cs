using System.Data;
using Dapper;
using OwlStream.Domain.Models.Countries;
using OwlStream.Domain.Models.Genres;
using OwlStream.Domain.Models.Languages;
using OwlStream.Domain.Models.Movies;
using OwlStream.Domain.Repositories;

namespace OwlStream.Infra.Repositories;

public class MoviesRepository : BaseRepository, IMoviesRepository
{
    public MoviesRepository(string connectionString) : base(connectionString) { }

    private async Task LinkGenresToMovie(string movieId, IEnumerable<int> genresIds)
    {
        // Insert command
        string sql = "INSERT INTO MoviesGenres VALUES(@MovieId, @GenreId)";
        using var connection = await CreateConnection();

        // Execute insert command
        foreach (var genreId in genresIds)
        {
            await connection.ExecuteAsync(sql, new
            {
                MovieId = movieId,
                GenreId = genreId
            });
        }
    }

    private async Task UnlinkGenresFromMovie(string movieId)
    {
        // Delete MoviesGenres that contains Movie Id
        var sql = $"DELETE MoviesGenres WHERE [MovieId] = {movieId}";

        using var connection = await CreateConnection();
        await connection.ExecuteAsync(sql);
    }

    public async Task<IEnumerable<MovieResult>> Search(string text, int? genreId, string cinelistId, int limit)
    {
        // 1. Returns all Movies that matches search text
        // 2. Returns all Genres from specific movie
        string[] sql = {
                @"
                SELECT TOP(@Limit) * FROM Movies m
                WHERE [Title] LIKE @Text AND
                    [Active] = 1 AND
                    (@CinelistId IS NULL OR m.[Id] = (SELECT TOP 1 cm.[MovieId] FROM CinelistsMovies cm WHERE cm.[CinelistId] = @CinelistId AND cm.[MovieId] = m.[Id])) AND
                    (@GenreId    IS NULL OR m.[Id] = (SELECT TOP 1 mg.[MovieId] FROM MoviesGenres    mg WHERE mg.[GenreId]    = @GenreId    AND mg.[MovieId] = m.[Id]))",
                "SELECT * FROM MoviesGenres mg INNER JOIN Genres g ON g.[Id] = mg.[GenreId] WHERE g.[Active] = 1 AND mg.[MovieId] = @Id",
                "SELECT * FROM MoviesCountries mc INNER JOIN Countries c ON c.[Id] = mc.[CountryId] WHERE mc.[MovieId] = @Id",
                "SELECT l.*, ml.[Main] FROM MoviesLanguages ml INNER JOIN Languages l ON l.[Id] = ml.[LanguageId] WHERE l.[Active] = 1 AND ml.[MovieId] = @Id"
            };

        using var connection = await CreateConnection();

        var movies = await connection.QueryAsync<MovieResult>(
            sql[0],
            new
            {
                Text = $"%{text}%",
                GenreId = genreId,
                CinelistId = cinelistId,
                Limit = limit
            }
        );

        movies = movies.Distinct();

        foreach (var movie in movies)
        {
            movie.Genres = await connection.QueryAsync<GenreResult>(
                sql[1],
                new
                {
                    movie.Id
                }
            );

            movie.Countries = await connection.QueryAsync<CountryResult>(
                sql[2],
                new
                {
                    movie.Id
                }
            );

            movie.Languages = await connection.QueryAsync<LanguageResult>(
                sql[3],
                new
                {
                    movie.Id
                }
            );
        }

        return movies;
    }

    public async Task<MovieResult> Get(string id)
    {
        string sql = "SELECT TOP 1 * FROM Movies WHERE [Id] = @Id";

        using var connection = await CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<MovieResult>(sql, new { Id = id });
    }

    public async Task<string> Add(MovieAdd movie)
    {
        // 1. Insert Movie
        // 2. Returns Id from last created Movie
        string[] sql = {
                "INSERT INTO Movies([Title], [LaunchDate], [Synopsis], [DurationInMinutes]) VALUES(@Title, @LaunchDate, @Synopsis, @DurationInMinutes)",
                "SELECT [Id] FROM Movies ORDER BY CreationDate DESC"
            };

        using var connection = await CreateConnection();

        var result = await connection.ExecuteAsync(
            sql[0],
            new
            {
                movie.Title,
                movie.LaunchDate,
                movie.Synopsis,
                movie.DurationInMinutes
            });

        if (result == 1)
        {
            var newId = await connection.QueryFirstOrDefaultAsync<string>(sql[1]);
            var genresResult = LinkGenresToMovie(newId, movie.Genres.Select(g => g.Id));

            return newId;
        }

        return null;
    }

    public async Task<bool> Update(MovieUpdate movie)
    {
        // Update Movie
        var sql = @"
                UPDATE Movies
                SET [Title] = @Title,
                    [LaunchDate] = @LaunchDate,
                    [Synopsis] = @Synopsis,
                    [DurationInMinutes] = @DurationInMinutes
                WHERE [Id] = @Id";

        using var connection = await CreateConnection();

        var result = await connection.ExecuteAsync(
            sql,
            new
            {
                movie.Id,
                movie.Title,
                movie.Synopsis,
                movie.LaunchDate,
                movie.DurationInMinutes
            });

        if (result == 1)
        {
            await UnlinkGenresFromMovie(movie.Id);
            await LinkGenresToMovie(movie.Id, movie.Genres.Select(g => g.Id));

            return true;
        }

        return false;
    }

    public async Task<bool> ChangeStatus(string id, bool status)
    {
        var sql = "UPDATE Movies SET [Active] = @Active WHERE [Id] = @Id";

        using var connection = await CreateConnection();

        var result = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            Active = status
        });

        return result == 1;
    }

    public async Task<bool> AddUrl(string id, string column, string url)
    {
        var sql = $"UPDATE Movies SET [{column}] = @Url WHERE [Id] = @Id";

        using var connection = await CreateConnection();

        var result = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            Column = column,
            Url = url
        });

        return result == 1;
    }
}