using JobBot.Infrastructure.Persistence.Connection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobBot.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'PostgreSQL' не найдена.");

        services.AddSingleton<IDbConnectionFactory>(
            new NpgsqlConnectionFactory(connectionString));

        return services;
    }
}