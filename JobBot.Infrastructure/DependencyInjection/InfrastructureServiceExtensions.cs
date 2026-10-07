using JobBot.Application.Abstractions.Repositories;
using JobBot.Infrastructure.Persistence.Connection;
using JobBot.Infrastructure.Persistence.Migrations;
using JobBot.Infrastructure.Persistence.Repositories;
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
        
        services.AddScoped<MigrationRunner>();
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IResumeRepository, ResumeRepository>();
        services.AddScoped<ISearchFilterRepository, SearchFilterRepository>();

        services.AddScoped<IVacancyRepository, VacancyRepository>();
        services.AddScoped<IApplicationRepository, JobApplicationRepository>();
        services.AddScoped<IPlatformRepository, PlatformRepository>();
        services.AddScoped<IPlatformAccountRepository, PlatformAccountRepository>();
        
        return services;
    }
}