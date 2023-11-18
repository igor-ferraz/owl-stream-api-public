using Dapper;
using OwlStream.Domain.Models.Cast;
using OwlStream.Domain.Repositories;

namespace OwlStream.Infra.Repositories;

public class CastRepository : BaseRepository, ICastRepository
{
    private readonly IPeopleRepository _peopleRepository;

    public CastRepository(string connectionString, IPeopleRepository peopleRepository) : base(connectionString)
    {
        _peopleRepository = peopleRepository;
    }

    public async Task<CastResult> Get(string id)
    {
        string[] sql = {
                @"SELECT
                    [Id],
                    [Name],
                    [Surname],
                    [Birthdate],
                    [CreationDate],
                    [GenderId]
                FROM [People] WHERE [Id] = @Id",
                @"SELECT
                    m.[Title] AS [Movie],
                    pm.[MovieId],
                    r.[Description] AS [Role],
                    pm.[RoleId],
                    pm.[Character]
                FROM PeopleMovies pm
                    INNER JOIN Movies m ON pm.[MovieId] = m.[Id]
                    INNER JOIN Roles r ON r.[Id] = pm.[RoleId]
                WHERE pm.[PersonId] = @PersonId"
            };

        using var connection = await CreateConnection();

        var person = await connection.QueryFirstOrDefaultAsync<CastResult>(sql[0], new
        {
            Id = id
        });

        if (person is not null)
        {
            // person.Movies = new List<CastMovieResult>();

            person.Movies = await connection.QueryAsync<CastMovieResult>(sql[1], new
            {
                PersonId = id
            });

            return person;
        }

        return null;
    }

    public async Task<string> Add(CastAdd person)
    {
        // Inserts PeopleMovies
        var insertSql = "INSERT INTO PeopleMovies([MovieId], [PersonId], [RoleId], [Character]) VALUES(@MovieId, @PersonId, @RoleId, @Character)";

        // Deletes PeopleMovies
        var rollbackSql = "DELETE FROM PeopleMovies WHERE [MovieId] = @MovieId AND [PersonId] = @PersonId AND [RoleId] = @RoleId";

        var personId = await _peopleRepository.Add(person);

        if (String.IsNullOrEmpty(personId))
        {
            return null;
        }

        // Link people to movies
        if (person.Movies is not null && person.Movies.Any())
        {
            using var connection = await CreateConnection();
            var result = 0;

            foreach (var movie in person.Movies)
            {
                result += await connection.ExecuteAsync(insertSql, new
                {
                    movie.MovieId,
                    PersonId = personId,
                    movie.RoleId,
                    movie.Character
                });
            }

            if (result != person.Movies.Count)
            {
                foreach (var movie in person.Movies)
                {
                    await connection.ExecuteAsync(rollbackSql, new
                    {
                        movie.MovieId,
                        PersonId = personId,
                        movie.RoleId
                    });
                }

                await _peopleRepository.Delete(personId);

                return null;
            }
        }

        return personId;
    }

    public async Task<bool> Update(CastUpdate person)
    {
        // Update person
        // var sql = @"
        //     UPDATE Cast
        //     SET [Description] = @Description,
        //         [Active] = @Active
        //     WHERE [Id] = @Id";
        // var updateSql =
        //     @"UPDATE People
        //     SET [Name] = @Name,
        //         [Surname] = @Surname,
        //         [Birthdate] = @Birthdate,
        //         [GenderId] = @GenderId
        //     WHERE [Id] = @PersonId";

        // // Deletes PeopleMovies
        // var rollbackSql = "DELETE FROM PeopleMovies WHERE [MovieId] = @MovieId AND [PersonId] = @PersonId AND [RoleId] = @RoleId";

        // var personId = await _peopleRepository.Add(person);

        // if (String.IsNullOrEmpty(personId))
        // {
        //     return null;
        // }

        // // Link people to movies
        // if (person.Movies is not null && person.Movies.Any())
        // {
        //     using var connection = await _CreateConnection();
        //     var result = 0;

        //     foreach (var movie in person.Movies)
        //     {
        //         result += await connection.ExecuteAsync(insertSql, new
        //         {
        //             movie.MovieId,
        //             PersonId = personId,
        //             movie.RoleId,
        //             movie.Character
        //         });
        //     }

        //     if (result != person.Movies.Count)
        //     {
        //         foreach (var movie in person.Movies)
        //         {
        //             await connection.ExecuteAsync(rollbackSql, new
        //             {
        //                movie.MovieId,
        //                PersonId = personId,
        //                movie.RoleId 
        //             });
        //         }

        //         await _peopleRepository.Delete(personId);

        //         return null;
        //     }
        // }

        // return personId;
        var cast = await Get(string.Empty);

        return true;
    }

    public async Task<bool> Delete(string id)
    {
        var sql = "DELETE PeopleMovies WHERE [PersonId] = @Id";

        using var connection = await CreateConnection();

        var result = await connection.ExecuteAsync(
            sql,
            new
            {
                Id = id
            });

        if (result == 1)
        {
            await _peopleRepository.Delete(id);
        }

        return result == 1;
    }
}