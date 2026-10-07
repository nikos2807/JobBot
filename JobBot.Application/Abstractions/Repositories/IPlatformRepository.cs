using JobBot.Domain.Entities;

namespace JobBot.Application.Abstractions.Repositories;

public interface IPlatformRepository
{
    Task<Platform?> GetByIdAsync(long id);

    Task<Platform?> GetByTypeAsync(
        Domain.Enums.PlatformType type);

    Task AddAsync(Platform platform);

    Task UpdateAsync(Platform platform);
}