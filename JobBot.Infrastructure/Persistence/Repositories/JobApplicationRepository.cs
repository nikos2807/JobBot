using JobBot.Application.Abstractions.Repositories;
using JobBot.Domain.Entities;
using JobBot.Domain.Enums;
using JobBot.Infrastructure.Persistence.Connection;
using Npgsql;

namespace JobBot.Infrastructure.Persistence.Repositories;

public class JobApplicationRepository(
    IDbConnectionFactory connectionFactory) : IApplicationRepository
{
    public async Task<JobApplication?> GetByIdAsync(long id)
    {
        const string sql = """
            SELECT
                id,
                user_id,
                vacancy_id,
                status,
                external_application_id,
                error_message,
                sent_at,
                created_at,
                updated_at
            FROM job_applications
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

        return MapApplication(reader);
    }

    public async Task<JobApplication?> GetByUserAndVacancyAsync(
        long userId,
        long vacancyId)
    {
        const string sql = """
            SELECT
                id,
                user_id,
                vacancy_id,
                status,
                external_application_id,
                error_message,
                sent_at,
                created_at,
                updated_at
            FROM job_applications
            WHERE user_id = @userId
              AND vacancy_id = @vacancyId;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@vacancyId", vacancyId);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapApplication(reader);
    }

    public async Task AddAsync(JobApplication application)
    {
        const string sql = """
            INSERT INTO job_applications
            (
                user_id,
                vacancy_id,
                status,
                external_application_id,
                error_message,
                sent_at,
                created_at,
                updated_at
            )
            VALUES
            (
                @userId,
                @vacancyId,
                @status,
                @externalApplicationId,
                @errorMessage,
                @sentAt,
                @createdAt,
                @updatedAt
            )
            RETURNING id;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@userId", application.UserId);
        command.Parameters.AddWithValue("@vacancyId", application.VacancyId);
        command.Parameters.AddWithValue(
            "@status",
            (int)application.Status);
        command.Parameters.AddWithValue(
            "@externalApplicationId",
            (object?)application.ExternalApplicationId ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@errorMessage",
            (object?)application.ErrorMessage ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@sentAt",
            (object?)application.SentAt ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@createdAt",
            application.CreatedAt);
        command.Parameters.AddWithValue(
            "@updatedAt",
            (object?)application.UpdatedAt ?? DBNull.Value);

        application.Id = Convert.ToInt64(
            await command.ExecuteScalarAsync());
    }

    public async Task UpdateAsync(JobApplication application)
    {
        const string sql = """
            UPDATE job_applications
            SET
                status = @status,
                external_application_id = @externalApplicationId,
                error_message = @errorMessage,
                sent_at = @sentAt,
                updated_at = @updatedAt
            WHERE id = @id;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", application.Id);
        command.Parameters.AddWithValue(
            "@status",
            (int)application.Status);
        command.Parameters.AddWithValue(
            "@externalApplicationId",
            (object?)application.ExternalApplicationId ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@errorMessage",
            (object?)application.ErrorMessage ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@sentAt",
            (object?)application.SentAt ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@updatedAt",
            (object?)application.UpdatedAt ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<int> CountTodayByUserIdAsync(long userId)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM job_applications
            WHERE user_id = @userId
              AND created_at >= CURRENT_DATE
              AND created_at < CURRENT_DATE + INTERVAL '1 day';
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@userId", userId);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    private static JobApplication MapApplication(
        NpgsqlDataReader reader)
    {
        return new JobApplication
        {
            Id = reader.GetInt64(reader.GetOrdinal("id")),

            UserId = reader.GetInt64(
                reader.GetOrdinal("user_id")),

            VacancyId = reader.GetInt64(
                reader.GetOrdinal("vacancy_id")),

            Status = (ApplicationStatus)reader.GetInt32(
                reader.GetOrdinal("status")),

            ExternalApplicationId =
                reader.IsDBNull(
                    reader.GetOrdinal("external_application_id"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("external_application_id")),

            ErrorMessage =
                reader.IsDBNull(
                    reader.GetOrdinal("error_message"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("error_message")),

            SentAt =
                reader.IsDBNull(reader.GetOrdinal("sent_at"))
                    ? null
                    : reader.GetDateTime(
                        reader.GetOrdinal("sent_at")),

            CreatedAt = reader.GetDateTime(
                reader.GetOrdinal("created_at")),

            UpdatedAt =
                reader.IsDBNull(reader.GetOrdinal("updated_at"))
                    ? null
                    : reader.GetDateTime(
                        reader.GetOrdinal("updated_at"))
        };
    }
}