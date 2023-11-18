using Dapper;
using OwlStream.Domain.Models.Genres;
using OwlStream.Domain.Repositories;

namespace OwlStream.Infra.Repositories;

public class GenresRepository : BaseRepository, IGenresRepository
{
    public GenresRepository(string connectionString) : base(connectionString) { }

    public async Task<IEnumerable<GenreResult>> Get(bool mainOnly, bool activeOnly)
    {
        // Returns all genres if @MainOnly is false
        var sql = "SELECT * FROM Genres WHERE (@MainOnly = 0 OR [Main] = @MainOnly) AND (@ActiveOnly = 0 OR [Active] = @ActiveOnly)";

        using var connection = await CreateConnection();

        return await connection.QueryAsync<GenreResult>(
            sql,
            new
            {
                MainOnly = mainOnly,
                ActiveOnly = activeOnly
            }
        );
    }

    public async Task<string> Add(Genre genre)
    {
        // 1. Insert genre
        // 2. Returns ld from last created genre
        string[] sql = {
                "INSERT INTO Genres([Description], [Main]) VALUES(@Description, @Main)",
                "SELECT [Id] FROM Genres ORDER BY [CreationDate] DESC"
            };

        using var connection = await CreateConnection();

        var result = await connection.ExecuteAsync(
            sql[0],
            new
            {
                genre.Description,
                genre.Main
            }
        );

        if (result == 1)
        {
            return await connection.QueryFirstOrDefaultAsync<string>(sql[1]);
        }

        return null;
    }

    public async Task<bool> Update(GenreUpdate genre)
    {
        // Update genre
        var sql = @"
                UPDATE Genres
                SET [Description] = @Description,
                    [Main] = @Main
                WHERE [Id] = @Id";

        using var connection = await CreateConnection();

        var result = await connection.ExecuteAsync(
            sql,
            new
            {
                genre.Id,
                genre.Description,
                genre.Main
            });

        return result == 1;
    }

    public async Task<bool> ChangeStatus(string id, bool status)
    {
        var sql = "UPDATE Genres SET [Active] = @Status WHERE [Id] = @Id";

        using var connection = await CreateConnection();

        var result = await connection.ExecuteAsync(
            sql,
            new
            {
                Id = id,
                Status = status
            });

        return result == 1;
    }
}