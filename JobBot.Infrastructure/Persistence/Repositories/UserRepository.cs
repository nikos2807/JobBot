using JobBot.Application.Abstractions.Repositories;
using JobBot.Domain.Entities;
using JobBot.Domain.Enums;
using JobBot.Infrastructure.Persistence.Connection;

namespace JobBot.Infrastructure.Persistence.Repositories;

public class UserRepository(
    IDbConnectionFactory connectionFactory) : IUserRepository
{
    public async Task<User?> GetByIdAsync(long id)
    {
        const string sql = """
            SELECT
                id,
                telegram_id,
                username,
                first_name,
                last_name,
                status,
                created_at,
                updated_at
            FROM users
            WHERE id = @id;
            """;

        using var connection = connectionFactory.CreateConnection();
        await ((Npgsql.NpgsqlConnection)connection).OpenAsync();

        using var command = connection.CreateCommand();

        command.CommandText = sql;

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@id";
        parameter.Value = id;

        command.Parameters.Add(parameter);

        await using var reader =
            await ((Npgsql.NpgsqlCommand)command).ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapUser(reader);
    }

    public async Task<User?> GetByTelegramIdAsync(long telegramId)
    {
        const string sql = """
            SELECT
                id,
                telegram_id,
                username,
                first_name,
                last_name,
                status,
                created_at,
                updated_at
            FROM users
            WHERE telegram_id = @telegramId;
            """;

        using var connection = connectionFactory.CreateConnection();
        await ((Npgsql.NpgsqlConnection)connection).OpenAsync();

        using var command = connection.CreateCommand();

        command.CommandText = sql;

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@telegramId";
        parameter.Value = telegramId;

        command.Parameters.Add(parameter);

        await using var reader =
            await ((Npgsql.NpgsqlCommand)command).ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapUser(reader);
    }

    public Task AddAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(User user)
    {
        throw new NotImplementedException();
    }

    private static User MapUser(Npgsql.NpgsqlDataReader reader)
    {
        return new User
        {
            Id = reader.GetInt64(reader.GetOrdinal("id")),
            TelegramId = reader.GetInt64(reader.GetOrdinal("telegram_id")),
            Username = reader.IsDBNull(reader.GetOrdinal("username"))
                ? null
                : reader.GetString(reader.GetOrdinal("username")),
            FirstName = reader.IsDBNull(reader.GetOrdinal("first_name"))
                ? null
                : reader.GetString(reader.GetOrdinal("first_name")),
            LastName = reader.IsDBNull(reader.GetOrdinal("last_name"))
                ? null
                : reader.GetString(reader.GetOrdinal("last_name")),
            Status = (UserStatus)reader.GetInt32(reader.GetOrdinal("status")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("updated_at"))
        };
    }
}