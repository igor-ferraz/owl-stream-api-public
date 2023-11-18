using System.Data;
using System.Data.SqlClient;

namespace OwlStream.Infra.Repositories;

public abstract class BaseRepository
{
    private readonly string _connectionString;

    public BaseRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IDbConnection> CreateConnection()
    {
        try
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
        catch (TimeoutException ex)
        {
            throw new Exception(String.Format("{0}.CreateConnection() experienced a SQL timeout", GetType().FullName), ex);
        }
        catch (SqlException ex)
        {
            throw new Exception(String.Format("{0}.CreateConnection() experienced a SQL exception (not a timeout)", GetType().FullName), ex);
        }
    }
}