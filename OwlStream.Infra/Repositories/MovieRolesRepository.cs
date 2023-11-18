using Dapper;
using OwlStream.Domain.Models.MovieRoles;
using OwlStream.Domain.Repositories;

namespace OwlStream.Infra.Repositories;

public class MovieRolesRepository : BaseRepository, IMovieRolesRepository
{
    public MovieRolesRepository(string connectionString) : base(connectionString) { }

    public async Task<IEnumerable<MovieRoleResult>> Get(bool activeOnly)
    {
        // Returns all movie roles if @MainOnly is false
        var sql = "SELECT * FROM MovieRoles WHERE (@ActiveOnly = 0 OR [Active] = @ActiveOnly)";

        using var connection = await CreateConnection();

        return await connection.QueryAsync<MovieRoleResult>(
            sql,
            new
            {
                ActiveOnly = activeOnly
            }
        );
    }

    public async Task<string> Add(MovieRole movieRole)
    {
        // 1. Insert movie role
        // 2. Returns ld from last created movie role
        string[] sql = {
                "INSERT INTO MovieRoles([Description]) VALUES(@Description)",
                "SELECT [Id] FROM MovieRoles ORDER BY [CreationDate] DESC"
            };

        using var connection = await CreateConnection();

        var result = await connection.ExecuteAsync(
            sql[0],
            new
            {
                movieRole.Description
            }
        );

        if (result == 1)
        {
            return await connection.QueryFirstOrDefaultAsync<string>(sql[1]);
        }

        return null;
    }

    public async Task<bool> Update(MovieRoleUpdate movieRole)
    {
        // Update movie role
        var sql = @"
                UPDATE MovieRoles
                SET [Description] = @Description
                WHERE [Id] = @Id";

        using var connection = await CreateConnection();

        var result = await connection.ExecuteAsync(
            sql,
            new
            {
                movieRole.Id,
                movieRole.Description
            });

        return result == 1;
    }

    public async Task<bool> ChangeStatus(string id, bool status)
    {
        var sql = "UPDATE MovieRoles SET [Active] = @Active WHERE [Id] = @Id";

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
}