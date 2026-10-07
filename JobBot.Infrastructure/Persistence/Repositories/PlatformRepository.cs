using JobBot.Application.Abstractions.Repositories;
using JobBot.Domain.Entities;
using JobBot.Domain.Enums;
using JobBot.Infrastructure.Persistence.Connection;
using Npgsql;

namespace JobBot.Infrastructure.Persistence.Repositories;

public class PlatformRepository(
    IDbConnectionFactory connectionFactory) : IPlatformRepository
{
    public async Task<Platform?> GetByIdAsync(long id)
    {
        const string sql = """
            SELECT
                id,
                type,
                name,
                is_active,
                created_at,
                updated_at
            FROM platforms
            WHERE id = @id;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapPlatform(reader);
    }

    public async Task<Platform?> GetByTypeAsync(PlatformType type)
    {
        const string sql = """
            SELECT
                id,
                type,
                name,
                is_active,
                created_at,
                updated_at
            FROM platforms
            WHERE type = @type;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@type", (int)type);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapPlatform(reader);
    }

    public async Task AddAsync(Platform platform)
    {
        const string sql = """
            INSERT INTO platforms
            (
                type,
                name,
                is_active,
                created_at,
                updated_at
            )
            VALUES
            (
                @type,
                @name,
                @isActive,
                @createdAt,
                @updatedAt
            )
            RETURNING id;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@type",
            (int)platform.Type);

        command.Parameters.AddWithValue(
            "@name",
            platform.Name);

        command.Parameters.AddWithValue(
            "@isActive",
            platform.IsActive);

        command.Parameters.AddWithValue(
            "@createdAt",
            platform.CreatedAt);

        command.Parameters.AddWithValue(
            "@updatedAt",
            (object?)platform.UpdatedAt ?? DBNull.Value);

        platform.Id = Convert.ToInt64(
            await command.ExecuteScalarAsync());
    }

    public async Task UpdateAsync(Platform platform)
    {
        const string sql = """
            UPDATE platforms
            SET
                type = @type,
                name = @name,
                is_active = @isActive,
                updated_at = @updatedAt
            WHERE id = @id;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@id",
            platform.Id);

        command.Parameters.AddWithValue(
            "@type",
            (int)platform.Type);

        command.Parameters.AddWithValue(
            "@name",
            platform.Name);

        command.Parameters.AddWithValue(
            "@isActive",
            platform.IsActive);

        command.Parameters.AddWithValue(
            "@updatedAt",
            (object?)platform.UpdatedAt ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    private static Platform MapPlatform(
        NpgsqlDataReader reader)
    {
        return new Platform
        {
            Id = reader.GetInt64(
                reader.GetOrdinal("id")),

            Type = (PlatformType)reader.GetInt32(
                reader.GetOrdinal("type")),

            Name = reader.GetString(
                reader.GetOrdinal("name")),

            IsActive = reader.GetBoolean(
                reader.GetOrdinal("is_active")),

            CreatedAt = reader.GetDateTime(
                reader.GetOrdinal("created_at")),

            UpdatedAt = reader.IsDBNull(
                reader.GetOrdinal("updated_at"))
                ? null
                : reader.GetDateTime(
                    reader.GetOrdinal("updated_at"))
        };
    }
}