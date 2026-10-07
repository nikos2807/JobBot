using JobBot.Application.Abstractions.Repositories;
using JobBot.Domain.Entities;
using JobBot.Infrastructure.Persistence.Connection;
using Npgsql;

namespace JobBot.Infrastructure.Persistence.Repositories;

public class SearchFilterRepository(
    IDbConnectionFactory connectionFactory) : ISearchFilterRepository
{
    public async Task<SearchFilter?> GetByUserIdAsync(long userId)
    {
        const string sql = """
            SELECT
                id,
                user_id,
                keywords,
                salary_from,
                salary_to,
                location,
                remote_only,
                employment_type,
                is_active,
                created_at,
                updated_at
            FROM search_filters
            WHERE user_id = @userId
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

        return MapSearchFilter(reader);
    }

    public async Task AddAsync(SearchFilter filter)
    {
        const string sql = """
            INSERT INTO search_filters
            (
                user_id,
                keywords,
                salary_from,
                salary_to,
                location,
                remote_only,
                employment_type,
                is_active,
                created_at,
                updated_at
            )
            VALUES
            (
                @userId,
                @keywords,
                @salaryFrom,
                @salaryTo,
                @location,
                @remoteOnly,
                @employmentType,
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

        command.Parameters.AddWithValue("@userId", filter.UserId);
        command.Parameters.AddWithValue(
            "@keywords",
            (object?)filter.Keywords ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@salaryFrom",
            (object?)filter.SalaryFrom ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@salaryTo",
            (object?)filter.SalaryTo ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@location",
            (object?)filter.Location ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@remoteOnly",
            filter.RemoteOnly);
        command.Parameters.AddWithValue(
            "@employmentType",
            (object?)filter.EmploymentType ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@isActive",
            filter.IsActive);
        command.Parameters.AddWithValue(
            "@createdAt",
            filter.CreatedAt);
        command.Parameters.AddWithValue(
            "@updatedAt",
            (object?)filter.UpdatedAt ?? DBNull.Value);

        filter.Id = Convert.ToInt64(
            await command.ExecuteScalarAsync());
    }

    public async Task UpdateAsync(SearchFilter filter)
    {
        const string sql = """
            UPDATE search_filters
            SET
                keywords = @keywords,
                salary_from = @salaryFrom,
                salary_to = @salaryTo,
                location = @location,
                remote_only = @remoteOnly,
                employment_type = @employmentType,
                is_active = @isActive,
                updated_at = @updatedAt
            WHERE id = @id;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", filter.Id);
        command.Parameters.AddWithValue(
            "@keywords",
            (object?)filter.Keywords ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@salaryFrom",
            (object?)filter.SalaryFrom ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@salaryTo",
            (object?)filter.SalaryTo ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@location",
            (object?)filter.Location ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@remoteOnly",
            filter.RemoteOnly);
        command.Parameters.AddWithValue(
            "@employmentType",
            (object?)filter.EmploymentType ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@isActive",
            filter.IsActive);
        command.Parameters.AddWithValue(
            "@updatedAt",
            (object?)filter.UpdatedAt ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    private static SearchFilter MapSearchFilter(
        NpgsqlDataReader reader)
    {
        return new SearchFilter
        {
            Id = reader.GetInt64(reader.GetOrdinal("id")),
            UserId = reader.GetInt64(reader.GetOrdinal("user_id")),

            Keywords = reader.IsDBNull(reader.GetOrdinal("keywords"))
                ? null
                : reader.GetString(reader.GetOrdinal("keywords")),

            SalaryFrom = reader.IsDBNull(reader.GetOrdinal("salary_from"))
                ? null
                : reader.GetDecimal(reader.GetOrdinal("salary_from")),

            SalaryTo = reader.IsDBNull(reader.GetOrdinal("salary_to"))
                ? null
                : reader.GetDecimal(reader.GetOrdinal("salary_to")),

            Location = reader.IsDBNull(reader.GetOrdinal("location"))
                ? null
                : reader.GetString(reader.GetOrdinal("location")),

            RemoteOnly = reader.GetBoolean(
                reader.GetOrdinal("remote_only")),

            EmploymentType = reader.IsDBNull(
                reader.GetOrdinal("employment_type"))
                ? null
                : reader.GetString(
                    reader.GetOrdinal("employment_type")),

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