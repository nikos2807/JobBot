using JobBot.Application.Abstractions.Repositories;
using JobBot.Domain.Entities;
using JobBot.Infrastructure.Persistence.Connection;
using Npgsql;

namespace JobBot.Infrastructure.Persistence.Repositories;

public class VacancyRepository(
    IDbConnectionFactory connectionFactory) : IVacancyRepository
{
    public async Task<Vacancy?> GetByIdAsync(long id)
    {
        const string sql = """
            SELECT
                id,
                platform_id,
                external_id,
                title,
                company_name,
                description,
                url,
                salary_from,
                salary_to,
                currency,
                location,
                remote,
                published_at,
                created_at,
                updated_at
            FROM vacancies
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

        return MapVacancy(reader);
    }

    public async Task<Vacancy?> GetByExternalIdAsync(
        long platformId,
        string externalId)
    {
        const string sql = """
            SELECT
                id,
                platform_id,
                external_id,
                title,
                company_name,
                description,
                url,
                salary_from,
                salary_to,
                currency,
                location,
                remote,
                published_at,
                created_at,
                updated_at
            FROM vacancies
            WHERE platform_id = @platformId
              AND external_id = @externalId;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@platformId", platformId);
        command.Parameters.AddWithValue("@externalId", externalId);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapVacancy(reader);
    }

    public async Task AddAsync(Vacancy vacancy)
    {
        const string sql = """
            INSERT INTO vacancies
            (
                platform_id,
                external_id,
                title,
                company_name,
                description,
                url,
                salary_from,
                salary_to,
                currency,
                location,
                remote,
                published_at,
                created_at,
                updated_at
            )
            VALUES
            (
                @platformId,
                @externalId,
                @title,
                @companyName,
                @description,
                @url,
                @salaryFrom,
                @salaryTo,
                @currency,
                @location,
                @remote,
                @publishedAt,
                @createdAt,
                @updatedAt
            )
            RETURNING id;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@platformId", vacancy.PlatformId);
        command.Parameters.AddWithValue("@externalId", vacancy.ExternalId);
        command.Parameters.AddWithValue("@title", vacancy.Title);
        command.Parameters.AddWithValue(
            "@companyName",
            (object?)vacancy.CompanyName ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@description",
            (object?)vacancy.Description ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@url",
            (object?)vacancy.Url ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@salaryFrom",
            (object?)vacancy.SalaryFrom ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@salaryTo",
            (object?)vacancy.SalaryTo ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@currency",
            (object?)vacancy.Currency ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@location",
            (object?)vacancy.Location ?? DBNull.Value);
        command.Parameters.AddWithValue("@remote", vacancy.Remote);
        command.Parameters.AddWithValue("@publishedAt", vacancy.PublishedAt);
        command.Parameters.AddWithValue("@createdAt", vacancy.CreatedAt);
        command.Parameters.AddWithValue(
            "@updatedAt",
            (object?)vacancy.UpdatedAt ?? DBNull.Value);

        vacancy.Id = Convert.ToInt64(
            await command.ExecuteScalarAsync());
    }

    public async Task UpdateAsync(Vacancy vacancy)
    {
        const string sql = """
            UPDATE vacancies
            SET
                title = @title,
                company_name = @companyName,
                description = @description,
                url = @url,
                salary_from = @salaryFrom,
                salary_to = @salaryTo,
                currency = @currency,
                location = @location,
                remote = @remote,
                published_at = @publishedAt,
                updated_at = @updatedAt
            WHERE id = @id;
            """;

        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", vacancy.Id);
        command.Parameters.AddWithValue("@title", vacancy.Title);
        command.Parameters.AddWithValue(
            "@companyName",
            (object?)vacancy.CompanyName ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@description",
            (object?)vacancy.Description ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@url",
            (object?)vacancy.Url ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@salaryFrom",
            (object?)vacancy.SalaryFrom ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@salaryTo",
            (object?)vacancy.SalaryTo ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@currency",
            (object?)vacancy.Currency ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@location",
            (object?)vacancy.Location ?? DBNull.Value);
        command.Parameters.AddWithValue("@remote", vacancy.Remote);
        command.Parameters.AddWithValue("@publishedAt", vacancy.PublishedAt);
        command.Parameters.AddWithValue("@updatedAt", vacancy.UpdatedAt);
        
        await command.ExecuteNonQueryAsync();
    }

    private static Vacancy MapVacancy(NpgsqlDataReader reader)
    {
        return new Vacancy
        {
            Id = reader.GetInt64(reader.GetOrdinal("id")),
            PlatformId = reader.GetInt64(reader.GetOrdinal("platform_id")),
            ExternalId = reader.GetString(reader.GetOrdinal("external_id")),
            Title = reader.GetString(reader.GetOrdinal("title")),
            CompanyName = reader.IsDBNull(reader.GetOrdinal("company_name"))
                ? null
                : reader.GetString(reader.GetOrdinal("company_name")),
            Description = reader.IsDBNull(reader.GetOrdinal("description"))
                ? null
                : reader.GetString(reader.GetOrdinal("description")),
            Url = reader.IsDBNull(reader.GetOrdinal("url"))
                ? null
                : reader.GetString(reader.GetOrdinal("url")),
            SalaryFrom = reader.IsDBNull(reader.GetOrdinal("salary_from"))
                ? null
                : reader.GetDecimal(reader.GetOrdinal("salary_from")),
            SalaryTo = reader.IsDBNull(reader.GetOrdinal("salary_to"))
                ? null
                : reader.GetDecimal(reader.GetOrdinal("salary_to")),
            Currency = reader.IsDBNull(reader.GetOrdinal("currency"))
                ? null
                : reader.GetString(reader.GetOrdinal("currency")),
            Location = reader.IsDBNull(reader.GetOrdinal("location"))
                ? null
                : reader.GetString(reader.GetOrdinal("location")),
            Remote = reader.GetBoolean(reader.GetOrdinal("remote")),
            PublishedAt = reader.GetDateTime(
                reader.GetOrdinal("published_at")),
            CreatedAt = reader.GetDateTime(
                reader.GetOrdinal("created_at")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("updated_at"))
        };
    }
}