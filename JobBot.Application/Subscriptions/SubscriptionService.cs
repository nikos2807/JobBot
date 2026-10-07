
using JobBot.Application.Abstractions.Repositories;
using JobBot.Domain.Entities;
using JobBot.Domain.Enums;

namespace JobBot.Application.Subscriptions;

public class SubscriptionService(
    ISubscriptionRepository subscriptionRepository)
{
    public Task<Subscription?> GetActiveAsync(long userId)
    {
        return subscriptionRepository.GetActiveByUserIdAsync(userId);
    }

    public bool IsActive(Subscription? subscription)
    {
        if (subscription is null)
            return false;

        return subscription.Status == SubscriptionStatus.Active
               && subscription.ExpiresAt > DateTime.UtcNow;
    }
}