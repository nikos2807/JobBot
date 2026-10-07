using JobBot.Application.Abstractions.Repositories;
using JobBot.Domain.Entities;
using JobBot.Domain.Enums;
using JobBot.Infrastructure.Persistence.Connection;
using Npgsql;

namespace JobBot.Infrastructure.Persistence.Repositories;

public class PaymentRepository(
    IDbConnectionFactory connectionFactory) : IPaymentRepository
{
    public async Task<Payment?> GetByIdAsync(long id)
    {
        const string sql = """
            SELECT
                id,
                user_id,
                subscription_plan_id,
                amount,
                currency,
                status,
                external_payment_id,
                created_at,
                paid_at
            FROM payments
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

        return MapPayment(reader);
    }

    public async Task<Payment?> GetByExternalIdAsync(
        string externalPaymentId)
    {
        const string sql = """
            SELECT
                id,
                user_id,
                subscription_plan_id,
                amount,
                currency,
                status,
                external_payment_id,
                created_at,
                paid_at
            FROM payments
            WHERE external_payment_id = @externalPaymentId;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@externalPaymentId",
            externalPaymentId);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapPayment(reader);
    }

    public async Task AddAsync(Payment payment)
    {
        const string sql = """
            INSERT INTO payments
            (
                user_id,
                subscription_plan_id,
                amount,
                currency,
                status,
                external_payment_id,
                created_at,
                paid_at
            )
            VALUES
            (
                @userId,
                @subscriptionPlanId,
                @amount,
                @currency,
                @status,
                @externalPaymentId,
                @createdAt,
                @paidAt
            )
            RETURNING id;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@userId", payment.UserId);
        command.Parameters.AddWithValue(
            "@subscriptionPlanId",
            payment.SubscriptionPlanId);
        command.Parameters.AddWithValue("@amount", payment.Amount);
        command.Parameters.AddWithValue("@currency", payment.Currency);
        command.Parameters.AddWithValue(
            "@status",
            (int)payment.Status);
        command.Parameters.AddWithValue(
            "@externalPaymentId",
            (object?)payment.ExternalPaymentId ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@createdAt",
            payment.CreatedAt);
        command.Parameters.AddWithValue(
            "@paidAt",
            (object?)payment.PaidAt ?? DBNull.Value);

        payment.Id = Convert.ToInt64(
            await command.ExecuteScalarAsync());
    }

    public async Task UpdateAsync(Payment payment)
    {
        const string sql = """
            UPDATE payments
            SET
                status = @status,
                external_payment_id = @externalPaymentId,
                paid_at = @paidAt
            WHERE id = @id;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", payment.Id);
        command.Parameters.AddWithValue(
            "@status",
            (int)payment.Status);
        command.Parameters.AddWithValue(
            "@externalPaymentId",
            (object?)payment.ExternalPaymentId ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@paidAt",
            (object?)payment.PaidAt ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    private static Payment MapPayment(NpgsqlDataReader reader)
    {
        return new Payment
        {
            Id = reader.GetInt64(reader.GetOrdinal("id")),
            UserId = reader.GetInt64(reader.GetOrdinal("user_id")),
            SubscriptionPlanId = reader.GetInt64(
                reader.GetOrdinal("subscription_plan_id")),
            Amount = reader.GetDecimal(reader.GetOrdinal("amount")),
            Currency = reader.GetString(reader.GetOrdinal("currency")),
            Status = (PaymentStatus)reader.GetInt32(
                reader.GetOrdinal("status")),
            ExternalPaymentId = reader.IsDBNull(
                reader.GetOrdinal("external_payment_id"))
                ? null
                : reader.GetString(
                    reader.GetOrdinal("external_payment_id")),
            CreatedAt = reader.GetDateTime(
                reader.GetOrdinal("created_at")),
            PaidAt = reader.IsDBNull(
                reader.GetOrdinal("paid_at"))
                ? null
                : reader.GetDateTime(
                    reader.GetOrdinal("paid_at"))
        };
    }
}