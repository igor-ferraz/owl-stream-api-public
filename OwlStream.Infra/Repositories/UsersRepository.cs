using Dapper;
using OwlStream.Domain.Exceptions.Infra;
using OwlStream.Domain.Models.Users;
using OwlStream.Domain.Models.People;
using OwlStream.Domain.Models.Security;
using OwlStream.Domain.Repositories;

namespace OwlStream.Infra.Repositories;

public class UsersRepository : BaseRepository, IUsersRepository
{
    private readonly IPeopleRepository _peopleRepository;

    public UsersRepository(string connectionString, IPeopleRepository peopleRepository) : base(connectionString)
    {
        _peopleRepository = peopleRepository;
    }

    public async Task<UserResult> Get(Dictionary<string, object> filters)
    {
        if (!filters.Any())
        {
            throw new EmptyFiltersException();
        }

        var columns = new string[] { "Id", "Email" };

        foreach (var column in columns)
        {
            if (!filters.ContainsKey(column))
            {
                filters.Add(column, null);
            }
        }

        // 1. Get User (based on filters)
        // 2. Get Person by PersonId
        // 3. Get Gender by GenderId
        string[] sql = {
                "SELECT * FROM Users WHERE [Active] = 1 AND (@Id IS NULL OR [Id] = @Id) AND (@Email IS NULL OR [Email] = @Email)",
                "SELECT * FROM People WHERE [Id] = @PersonId",
                "SELECT * FROM Gender WHERE [Id] = @GenderId"
            };

        var parameters = new DynamicParameters(filters);

        using var connection = await CreateConnection();

        var user = await connection.QueryFirstOrDefaultAsync<UserResult>(sql[0], parameters);

        if (user != null)
        {
            user.Person = await connection.QueryFirstOrDefaultAsync<PersonResult>(
                sql[1],
                new
                {
                    user.PersonId
                });

            if (user.Person != null)
            {
                user.Person.Gender = await connection.QueryFirstOrDefaultAsync<Gender>(
                    sql[2],
                    new
                    {
                        user.Person.GenderId
                    });
            }

            return user;
        }

        return null;
    }

    public async Task<string> Add(UserAdd user)
    {
        using var connection = await CreateConnection();

        // Check if email is already registered
        var validationSql = "SELECT 1 FROM Users WHERE [Email] = @Email AND [Active] = 1";
        var emailAlreadyUsed = await connection.QueryFirstOrDefaultAsync<bool>(validationSql, new { Email = user.Email });

        if (emailAlreadyUsed)
        {
            throw new DuplicatedEmailException();
        }
        else
        {
            // Inserts User and returns new Id
            var insertSql = @"
                        DECLARE @Ids TABLE(Id VARCHAR(36));
                    
                        INSERT INTO Users([Email], [Password], [PersonId], [RoleId])
                        OUTPUT inserted.Id INTO @Ids
                        VALUES(
                            @Email,
                            @Password,
                            @PersonId,
                            (SELECT TOP 1 [Id] FROM [UserRoles] WHERE [Name] LIKE 'Client')
                        );
                        
                        SELECT TOP 1 Id FROM @Ids;";

            var personId = await _peopleRepository.Add(user.Person);

            if (!String.IsNullOrEmpty(personId))
            {
                var userId = await connection.QueryFirstOrDefaultAsync<string>(
                    insertSql,
                    new
                    {
                        user.Email,
                        user.Password,
                        PersonId = personId
                    });

                if (!String.IsNullOrEmpty(userId))
                {
                    return userId;
                }
                else
                {
                    await _peopleRepository.Delete(personId);
                }
            }

            return null;
        }
    }

    public async Task<bool> Update(UserUpdateInternal user)
    {
        // Validate if user email is already taken
        var validationSql = "SELECT 1 FROM Users WHERE [Active] = 1 AND [Email] = @Email AND [Id] != @Id";

        // Update statements
        string[] updateSql = {
                @"UPDATE Users
                SET [Email] = @Email
                WHERE [Id] = @Id AND
                      [Active] = 1",

                @"UPDATE People
                SET [Name] = @Name,
                    [Surname] = @Surname,
                    [Birthdate] = @Birthdate,
                    [GenderId] = @GenderId
                WHERE [Id] = (SELECT [PersonId] FROM Users WHERE [Id] = @Id)"
            };

        using var connection = await CreateConnection();

        var emailAlreadyExists = await connection.QueryFirstOrDefaultAsync<bool>(validationSql, new
        {
            user.Email,
            user.Id
        });

        if (emailAlreadyExists)
        {
            throw new DuplicatedEmailException();
        }

        var result = await connection.ExecuteAsync(
            updateSql[0],
            new
            {
                user.Id,
                user.Email
            });

        if (result == 1)
        {
            result += await connection.ExecuteAsync(
                updateSql[1],
                new
                {
                    user.Id,
                    user.Person.Name,
                    user.Person.Surname,
                    user.Person.Birthdate,
                    user.Person.GenderId
                });
        }

        return result == 2;
    }

    public async Task<bool> Delete(string id)
    {
        var sql = "UPDATE Users SET [Active] = 0 WHERE [Id] = @Id";

        using var connection = await CreateConnection();

        var result = await connection.ExecuteAsync(
            sql,
            new
            {
                Id = id
            });

        return result == 1;
    }

    public async Task<SecurityUser> GetSecurityUser(string email)
    {
        var sql = @"SELECT
                            u.[Id],
                            u.[Password],
                            ur.[Name] AS [Role]
                        FROM Users u
                            INNER JOIN UserRoles ur ON ur.[Id] = u.[RoleId]
                        WHERE [Active] = 1 AND [Email] = @Email";

        using var connection = await CreateConnection();

        var user = await connection.QueryFirstOrDefaultAsync<SecurityUser>(sql, new
        {
            Email = email
        });

        return user;
    }
}