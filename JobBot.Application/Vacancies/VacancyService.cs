using JobBot.Application.Abstractions.Repositories;
using JobBot.Domain.Entities;

namespace JobBot.Application.Vacancies;

public class VacancyService(
    IVacancyRepository vacancyRepository)
{
    public Task<Vacancy?> GetByIdAsync(long id)
    {
        return vacancyRepository.GetByIdAsync(id);
    }

    public Task<Vacancy?> GetByExternalIdAsync(
        long platformId,
        string externalId)
    {
        return vacancyRepository.GetByExternalIdAsync(
            platformId,
            externalId);
    }

    public async Task<Vacancy> CreateAsync(
        long platformId,
        string externalId,
        string title,
        string? companyName,
        string? description,
        string? url,
        decimal? salaryFrom,
        decimal? salaryTo,
        string? currency,
        string? location,
        bool remote,
        DateTime publishedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(externalId);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        var existingVacancy =
            await vacancyRepository.GetByExternalIdAsync(
                platformId,
                externalId);

        if (existingVacancy is not null)
            return existingVacancy;

        var vacancy = new Vacancy
        {
            PlatformId = platformId,
            ExternalId = externalId,
            Title = title,
            CompanyName = companyName,
            Description = description,
            Url = url,
            SalaryFrom = salaryFrom,
            SalaryTo = salaryTo,
            Currency = currency,
            Location = location,
            Remote = remote,
            PublishedAt = publishedAt,
            CreatedAt = DateTime.UtcNow
        };

        await vacancyRepository.AddAsync(vacancy);

        return vacancy;
    }
}