using JobBot.Domain.Entities;

namespace JobBot.Application.Abstractions.Repositories;

public interface IPlatformAccountRepository
{
    Task<PlatformAccount?> GetByIdAsync(long id);

    Task<PlatformAccount?> GetByUserAndPlatformAsync(
        long userId,
        long platformId);

    Task AddAsync(PlatformAccount account);

    Task UpdateAsync(PlatformAccount account);
}