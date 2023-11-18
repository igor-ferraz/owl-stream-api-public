using Dapper;
using OwlStream.Domain.Models.Cinelists;
using OwlStream.Domain.Models.Movies;
using OwlStream.Domain.Repositories;

namespace OwlStream.Infra.Repositories;

public class CinelistsRepository : BaseRepository, ICinelistsRepository
{
    public CinelistsRepository(string connectionString) : base(connectionString) { }

    public async Task<IEnumerable<CinelistResult>> Get(bool activeOnly)
    {
        // Returns all cinelists if @MainOnly is false
        string[] sql = {
                "SELECT * FROM [Cinelists] WHERE (@ActiveOnly = 0 OR [Active] = @ActiveOnly)",
                @"SELECT m.*
                    FROM [CinelistsMovies] cm
                    INNER JOIN [Movies] m ON m.[Id] = cm.[MovieId]
                    WHERE cm.[CinelistId] = @CinelistId AND m.[Active] = 1"
        };

        using var connection = await CreateConnection();

        var cinelists = await connection.QueryAsync<CinelistResult>(
            sql[0],
            new
            {
                ActiveOnly = activeOnly
            }
        );

        foreach (var cinelist in cinelists)
        {
            cinelist.Movies = await connection.QueryAsync<MovieResult>(
                sql[1],
                new
                {
                    CinelistId = cinelist.Id
                }
            );
        }

        return cinelists;
    }

    public async Task<string> Add(Cinelist cinelist)
    {
        // 1. Insert cinelist
        // 2. Returns ld from last created cinelist
        string[] sql = {
                "INSERT INTO Cinelists([Name], [Description]) VALUES(@Name, @Description)",
                "SELECT [Id] FROM Cinelists ORDER BY [CreationDate] DESC"
            };

        using var connection = await CreateConnection();

        var result = await connection.ExecuteAsync(
            sql[0],
            new
            {
                cinelist.Description,
                cinelist.Name
            }
        );

        if (result == 1)
        {
            return await connection.QueryFirstOrDefaultAsync<string>(sql[1]);
        }

        return null;
    }

    public async Task<bool> Update(CinelistUpdate cinelist)
    {
        // Update cinelist
        var sql = @"
                UPDATE Cinelists
                SET [Name] = @Name,
                    [Description] = @Description
                WHERE [Id] = @Id";

        using var connection = await CreateConnection();

        var result = await connection.ExecuteAsync(
            sql,
            new
            {
                cinelist.Id,
                cinelist.Name,
                cinelist.Description
            });

        return result == 1;
    }

    public async Task<bool> ChangeStatus(string id, bool status)
    {
        var sql = "UPDATE Cinelists SET [Active] = @Active WHERE [Id] = @Id";

        using var connection = await CreateConnection();

        var result = await connection.ExecuteAsync(
            sql,
            new
            {
                Id = id,
                Active = status
            });

        return result == 1;
    }

    public async Task<bool> LinkMovie(string id, string movieId)
    {
        var sql = "INSERT INTO CinelistsMovies(CinelistId, MovieId) VALUES(@Id, @MovieId)";

        using var connection = await CreateConnection();

        var result = await connection.ExecuteAsync(
            sql,
            new
            {
                Id = id,
                MovieId = movieId
            });

        return result == 1;
    }
}