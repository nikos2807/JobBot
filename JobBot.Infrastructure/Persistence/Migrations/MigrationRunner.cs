using System.Reflection;
using Npgsql;
using JobBot.Infrastructure.Persistence.Connection;

namespace JobBot.Infrastructure.Persistence.Migrations;

public class MigrationRunner(
    IDbConnectionFactory connectionFactory)
{
    public async Task MigrateAsync()
    {
        await using var connection =
            (NpgsqlConnection)connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await CreateMigrationsTableAsync(connection);

        var migrations = GetMigrations();

        foreach (var migration in migrations)
        {
            if (await IsMigrationAppliedAsync(connection, migration.Name))
                continue;

            await ApplyMigrationAsync(
                connection,
                migration.Name,
                migration.Sql);
        }
    }

    private static async Task CreateMigrationsTableAsync(
        NpgsqlConnection connection)
    {
        await using var command = new NpgsqlCommand(
            """
            CREATE TABLE IF NOT EXISTS schema_migrations
            (
                version     VARCHAR(255) PRIMARY KEY,
                applied_at  TIMESTAMPTZ NOT NULL DEFAULT NOW()
            );
            """,
            connection);

        await command.ExecuteNonQueryAsync();
    }

    private static async Task<bool> IsMigrationAppliedAsync(
        NpgsqlConnection connection,
        string version)
    {
        await using var command = new NpgsqlCommand(
            """
            SELECT EXISTS
            (
                SELECT 1
                FROM schema_migrations
                WHERE version = @version
            );
            """,
            connection);

        command.Parameters.AddWithValue("version", version);

        var result = await command.ExecuteScalarAsync();

        return result is true;
    }

    private static async Task ApplyMigrationAsync(
        NpgsqlConnection connection,
        string version,
        string sql)
    {
        await using var transaction =
            await connection.BeginTransactionAsync();

        try
        {
            await using var command =
                new NpgsqlCommand(sql, connection, transaction);

            await command.ExecuteNonQueryAsync();

            await using var migrationCommand = new NpgsqlCommand(
                """
                INSERT INTO schema_migrations (version)
                VALUES (@version);
                """,
                connection,
                transaction);

            migrationCommand.Parameters.AddWithValue("version", version);

            await migrationCommand.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static IReadOnlyList<MigrationFile> GetMigrations()
    {
        var baseDirectory = FindDatabaseMigrationsDirectory();

        var files = Directory
            .GetFiles(baseDirectory, "*.sql")
            .OrderBy(Path.GetFileName)
            .Select(file => new MigrationFile(
                Path.GetFileNameWithoutExtension(file),
                File.ReadAllText(file)))
            .ToList();

        return files;
    }

    private static string FindDatabaseMigrationsDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var migrationsPath = Path.Combine(
                directory.FullName,
                "Database",
                "Migrations");

            if (Directory.Exists(migrationsPath))
                return migrationsPath;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Директория Database/Migrations не найдена.");
    }

    private sealed record MigrationFile(
        string Name,
        string Sql);
}