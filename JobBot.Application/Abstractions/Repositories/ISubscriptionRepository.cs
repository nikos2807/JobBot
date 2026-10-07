using JobBot.Domain.Entities;

namespace JobBot.Application.Abstractions.Repositories;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetActiveByUserIdAsync(long userId);

    Task<Subscription?> GetByIdAsync(long id);

    Task AddAsync(Subscription subscription);

    Task UpdateAsync(Subscription subscription);
}