using JobBot.Domain.Entities;

namespace JobBot.Application.Abstractions.Repositories;

public interface IVacancyRepository
{
    Task<Vacancy?> GetByIdAsync(long id);

    Task<Vacancy?> GetByExternalIdAsync(
        long platformId,
        string externalId);

    Task AddAsync(Vacancy vacancy);

    Task UpdateAsync(Vacancy vacancy);
}