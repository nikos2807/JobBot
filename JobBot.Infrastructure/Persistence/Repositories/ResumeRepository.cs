using JobBot.Application.Abstractions.Repositories;
using JobBot.Domain.Entities;
using JobBot.Infrastructure.Persistence.Connection;
using Npgsql;

namespace JobBot.Infrastructure.Persistence.Repositories;

public class ResumeRepository(
    IDbConnectionFactory connectionFactory) : IResumeRepository
{
    public async Task<Resume?> GetByIdAsync(long id)
    {
        const string sql = """
            SELECT
                id,
                user_id,
                file_name,
                file_url,
                text,
                parsed_data,
                is_active,
                created_at,
                updated_at
            FROM resumes
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

        return MapResume(reader);
    }

    public async Task<Resume?> GetActiveByUserIdAsync(long userId)
    {
        const string sql = """
            SELECT
                id,
                user_id,
                file_name,
                file_url,
                text,
                parsed_data,
                is_active,
                created_at,
                updated_at
            FROM resumes
            WHERE user_id = @userId
              AND is_active = TRUE
            ORDER BY created_at DESC
            LIMIT 1;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@userId", userId);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapResume(reader);
    }

    public async Task AddAsync(Resume resume)
    {
        const string sql = """
            INSERT INTO resumes
            (
                user_id,
                file_name,
                file_url,
                text,
                parsed_data,
                is_active,
                created_at,
                updated_at
            )
            VALUES
            (
                @userId,
                @fileName,
                @fileUrl,
                @text,
                @parsedData,
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

        command.Parameters.AddWithValue("@userId", resume.UserId);
        command.Parameters.AddWithValue("@fileName", resume.FileName);
        command.Parameters.AddWithValue(
            "@fileUrl",
            (object?)resume.FileUrl ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@text",
            (object?)resume.Text ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@parsedData",
            (object?)resume.ParsedData ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@isActive",
            resume.IsActive);
        command.Parameters.AddWithValue(
            "@createdAt",
            resume.CreatedAt);
        command.Parameters.AddWithValue(
            "@updatedAt",
            (object?)resume.UpdatedAt ?? DBNull.Value);

        resume.Id = Convert.ToInt64(
            await command.ExecuteScalarAsync());
    }

    public async Task UpdateAsync(Resume resume)
    {
        const string sql = """
            UPDATE resumes
            SET
                file_name = @fileName,
                file_url = @fileUrl,
                text = @text,
                parsed_data = @parsedData,
                is_active = @isActive,
                updated_at = @updatedAt
            WHERE id = @id;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", resume.Id);
        command.Parameters.AddWithValue("@fileName", resume.FileName);
        command.Parameters.AddWithValue(
            "@fileUrl",
            (object?)resume.FileUrl ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@text",
            (object?)resume.Text ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@parsedData",
            (object?)resume.ParsedData ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@isActive",
            resume.IsActive);
        command.Parameters.AddWithValue(
            "@updatedAt",
            (object?)resume.UpdatedAt ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    private static Resume MapResume(NpgsqlDataReader reader)
    {
        return new Resume
        {
            Id = reader.GetInt64(reader.GetOrdinal("id")),
            UserId = reader.GetInt64(reader.GetOrdinal("user_id")),
            FileName = reader.GetString(reader.GetOrdinal("file_name")),
            FileUrl = reader.IsDBNull(reader.GetOrdinal("file_url"))
                ? null
                : reader.GetString(reader.GetOrdinal("file_url")),
            Text = reader.IsDBNull(reader.GetOrdinal("text"))
                ? null
                : reader.GetString(reader.GetOrdinal("text")),
            ParsedData = reader.IsDBNull(reader.GetOrdinal("parsed_data"))
                ? null
                : reader.GetString(reader.GetOrdinal("parsed_data")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
            CreatedAt = reader.GetDateTime(
                reader.GetOrdinal("created_at")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at"))
                ? null
                : reader.GetDateTime(
                    reader.GetOrdinal("updated_at"))
        };
    }
}