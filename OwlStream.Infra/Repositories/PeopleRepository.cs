using Dapper;
using OwlStream.Domain.Models.People;
using OwlStream.Domain.Repositories;

namespace OwlStream.Infra.Repositories;

public class PeopleRepository : BaseRepository, IPeopleRepository
{
    public PeopleRepository(string connectionString) : base(connectionString) { }

    public async Task<string> Add(Person person)
    {
        var sql = @"DECLARE @Ids TABLE(Id VARCHAR(36));

                        INSERT INTO People([Name], [Surname], [Birthdate], [GenderId])
                        OUTPUT inserted.Id INTO @Ids(Id)
                        VALUES(@Name, @Surname, @Birthdate, @GenderId);
                    
                        SELECT TOP 1 Id FROM @Ids;";

        using var connection = await CreateConnection();

        var personId = await connection.QueryFirstOrDefaultAsync<string>(
                sql,
                new
                {
                    person.Name,
                    person.Surname,
                    person.Birthdate,
                    person.GenderId
                }
            );

        if (String.IsNullOrEmpty(personId))
        {
            return null;
        }

        return personId;
    }

    public async Task<bool> Delete(string id)
    {
        var sql = "DELETE People WHERE [Id] = @PersonId";
        using var connection = await CreateConnection();
        var result = await connection.ExecuteAsync(sql, new { PersonId = id });

        return result == 1;
    }
}