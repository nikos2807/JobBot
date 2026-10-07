using System.Data;

namespace JobBot.Infrastructure.Persistence.Connection;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}