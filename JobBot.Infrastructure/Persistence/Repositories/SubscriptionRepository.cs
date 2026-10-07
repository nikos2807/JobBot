using JobBot.Application.Abstractions.Repositories;
using JobBot.Domain.Entities;
using JobBot.Domain.Enums;
using JobBot.Infrastructure.Persistence.Connection;
using Npgsql;

namespace JobBot.Infrastructure.Persistence.Repositories;

public class SubscriptionRepository(
    IDbConnectionFactory connectionFactory) : ISubscriptionRepository
{
    public async Task<Subscription?> GetActiveByUserIdAsync(long userId)
    {
        const string sql = """
            SELECT
                id,
                user_id,
                subscription_plan_id,
                status,
                started_at,
                expires_at,
                created_at,
                updated_at
            FROM subscriptions
            WHERE user_id = @userId
              AND status = @status
              AND expires_at > NOW()
            ORDER BY expires_at DESC
            LIMIT 1;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue(
            "@status",
            (int)SubscriptionStatus.Active);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapSubscription(reader);
    }

    public async Task<Subscription?> GetByIdAsync(long id)
    {
        const string sql = """
            SELECT
                id,
                user_id,
                subscription_plan_id,
                status,
                started_at,
                expires_at,
                created_at,
                updated_at
            FROM subscriptions
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

        return MapSubscription(reader);
    }

    public async Task AddAsync(Subscription subscription)
    {
        const string sql = """
            INSERT INTO subscriptions
            (
                user_id,
                subscription_plan_id,
                status,
                started_at,
                expires_at,
                created_at,
                updated_at
            )
            VALUES
            (
                @userId,
                @subscriptionPlanId,
                @status,
                @startedAt,
                @expiresAt,
                @createdAt,
                @updatedAt
            )
            RETURNING id;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@userId", subscription.UserId);
        command.Parameters.AddWithValue(
            "@subscriptionPlanId",
            subscription.SubscriptionPlanId);
        command.Parameters.AddWithValue(
            "@status",
            (int)subscription.Status);
        command.Parameters.AddWithValue(
            "@startedAt",
            subscription.StartedAt);
        command.Parameters.AddWithValue(
            "@expiresAt",
            subscription.ExpiresAt);
        command.Parameters.AddWithValue(
            "@createdAt",
            subscription.CreatedAt);
        command.Parameters.AddWithValue(
            "@updatedAt",
            (object?)subscription.UpdatedAt ?? DBNull.Value);

        subscription.Id = Convert.ToInt64(
            await command.ExecuteScalarAsync());
    }

    public async Task UpdateAsync(Subscription subscription)
    {
        const string sql = """
            UPDATE subscriptions
            SET
                status = @status,
                started_at = @startedAt,
                expires_at = @expiresAt,
                updated_at = @updatedAt
            WHERE id = @id;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", subscription.Id);
        command.Parameters.AddWithValue(
            "@status",
            (int)subscription.Status);
        command.Parameters.AddWithValue(
            "@startedAt",
            subscription.StartedAt);
        command.Parameters.AddWithValue(
            "@expiresAt",
            subscription.ExpiresAt);
        command.Parameters.AddWithValue(
            "@updatedAt",
            (object?)subscription.UpdatedAt ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    private static Subscription MapSubscription(
        NpgsqlDataReader reader)
    {
        return new Subscription
        {
            Id = reader.GetInt64(reader.GetOrdinal("id")),
            UserId = reader.GetInt64(reader.GetOrdinal("user_id")),
            SubscriptionPlanId = reader.GetInt64(
                reader.GetOrdinal("subscription_plan_id")),
            Status = (SubscriptionStatus)reader.GetInt32(
                reader.GetOrdinal("status")),
            StartedAt = reader.GetDateTime(
                reader.GetOrdinal("started_at")),
            ExpiresAt = reader.GetDateTime(
                reader.GetOrdinal("expires_at")),
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