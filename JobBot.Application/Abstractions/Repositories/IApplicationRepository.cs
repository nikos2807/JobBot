using JobBot.Domain.Entities;

namespace JobBot.Application.Abstractions.Repositories;

public interface IApplicationRepository
{
    Task<Domain.Entities.JobApplication?> GetByIdAsync(long id);

    Task<Domain.Entities.JobApplication?> GetByUserAndVacancyAsync(
        long userId,
        long vacancyId);

    Task AddAsync(Domain.Entities.JobApplication application);

    Task UpdateAsync(Domain.Entities.JobApplication application);

    Task<int> CountTodayByUserIdAsync(long userId);
}