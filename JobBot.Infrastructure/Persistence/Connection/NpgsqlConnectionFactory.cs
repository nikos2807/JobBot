using System.Data;
using Npgsql;

namespace JobBot.Infrastructure.Persistence.Connection;

public class NpgsqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(connectionString);
    }
}