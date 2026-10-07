using JobBot.Application.Abstractions.Repositories;
using JobBot.Domain.Entities;
using JobBot.Infrastructure.Persistence.Connection;
using Npgsql;

namespace JobBot.Infrastructure.Persistence.Repositories;

public class PlatformAccountRepository(
    IDbConnectionFactory connectionFactory) : IPlatformAccountRepository
{
    public async Task<PlatformAccount?> GetByIdAsync(long id)
    {
        const string sql = """
            SELECT
                id,
                user_id,
                platform_id,
                external_user_id,
                access_token,
                refresh_token,
                token_expires_at,
                is_active,
                created_at,
                updated_at
            FROM platform_accounts
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

        return MapPlatformAccount(reader);
    }

    public async Task<PlatformAccount?> GetByUserAndPlatformAsync(
        long userId,
        long platformId)
    {
        const string sql = """
            SELECT
                id,
                user_id,
                platform_id,
                external_user_id,
                access_token,
                refresh_token,
                token_expires_at,
                is_active,
                created_at,
                updated_at
            FROM platform_accounts
            WHERE user_id = @userId
              AND platform_id = @platformId;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@platformId", platformId);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapPlatformAccount(reader);
    }

    public async Task AddAsync(PlatformAccount account)
    {
        const string sql = """
            INSERT INTO platform_accounts
            (
                user_id,
                platform_id,
                external_user_id,
                access_token,
                refresh_token,
                token_expires_at,
                is_active,
                created_at,
                updated_at
            )
            VALUES
            (
                @userId,
                @platformId,
                @externalUserId,
                @accessToken,
                @refreshToken,
                @tokenExpiresAt,
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

        command.Parameters.AddWithValue("@userId", account.UserId);
        command.Parameters.AddWithValue("@platformId", account.PlatformId);

        command.Parameters.AddWithValue(
            "@externalUserId",
            (object?)account.ExternalUserId ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@accessToken",
            (object?)account.AccessToken ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@refreshToken",
            (object?)account.RefreshToken ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@tokenExpiresAt",
            (object?)account.TokenExpiresAt ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@isActive",
            account.IsActive);

        command.Parameters.AddWithValue(
            "@createdAt",
            account.CreatedAt);

        command.Parameters.AddWithValue(
            "@updatedAt",
            (object?)account.UpdatedAt ?? DBNull.Value);

        account.Id = Convert.ToInt64(
            await command.ExecuteScalarAsync());
    }

    public async Task UpdateAsync(PlatformAccount account)
    {
        const string sql = """
            UPDATE platform_accounts
            SET
                external_user_id = @externalUserId,
                access_token = @accessToken,
                refresh_token = @refreshToken,
                token_expires_at = @tokenExpiresAt,
                is_active = @isActive,
                updated_at = @updatedAt
            WHERE id = @id;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", account.Id);

        command.Parameters.AddWithValue(
            "@externalUserId",
            (object?)account.ExternalUserId ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@accessToken",
            (object?)account.AccessToken ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@refreshToken",
            (object?)account.RefreshToken ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@tokenExpiresAt",
            (object?)account.TokenExpiresAt ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@isActive",
            account.IsActive);

        command.Parameters.AddWithValue(
            "@updatedAt",
            (object?)account.UpdatedAt ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    private static PlatformAccount MapPlatformAccount(
        NpgsqlDataReader reader)
    {
        return new PlatformAccount
        {
            Id = reader.GetInt64(
                reader.GetOrdinal("id")),

            UserId = reader.GetInt64(
                reader.GetOrdinal("user_id")),

            PlatformId = reader.GetInt64(
                reader.GetOrdinal("platform_id")),

            ExternalUserId = reader.IsDBNull(
                reader.GetOrdinal("external_user_id"))
                ? null
                : reader.GetString(
                    reader.GetOrdinal("external_user_id")),

            AccessToken = reader.IsDBNull(
                reader.GetOrdinal("access_token"))
                ? null
                : reader.GetString(
                    reader.GetOrdinal("access_token")),

            RefreshToken = reader.IsDBNull(
                reader.GetOrdinal("refresh_token"))
                ? null
                : reader.GetString(
                    reader.GetOrdinal("refresh_token")),

            TokenExpiresAt = reader.IsDBNull(
                reader.GetOrdinal("token_expires_at"))
                ? null
                : reader.GetDateTime(
                    reader.GetOrdinal("token_expires_at")),

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