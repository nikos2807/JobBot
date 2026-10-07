using JobBot.Application.Abstractions.Repositories;
using JobBot.Domain.Entities;
using JobBot.Domain.Enums;

namespace JobBot.Application.Platforms;

public class PlatformService(
    IPlatformRepository platformRepository)
{
    public Task<Platform?> GetByIdAsync(long id)
    {
        return platformRepository.GetByIdAsync(id);
    }

    public Task<Platform?> GetByTypeAsync(PlatformType type)
    {
        return platformRepository.GetByTypeAsync(type);
    }

    public async Task<Platform> CreateAsync(
        PlatformType type,
        string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var existingPlatform =
            await platformRepository.GetByTypeAsync(type);

        if (existingPlatform is not null)
            return existingPlatform;

        var platform = new Platform
        {
            Type = type,
            Name = name,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await platformRepository.AddAsync(platform);

        return platform;
    }
}