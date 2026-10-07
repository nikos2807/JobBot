using JobBot.Domain.Entities;

namespace JobBot.Application.Abstractions.Repositories;

public interface IResumeRepository
{
    Task<Resume?> GetByIdAsync(long id);

    Task<Resume?> GetActiveByUserIdAsync(long userId);

    Task AddAsync(Resume resume);

    Task UpdateAsync(Resume resume);
}