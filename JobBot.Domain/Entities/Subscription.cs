
namespace JobBot.Domain.Entities;

using JobBot.Domain.Enums;

public class Subscription
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long SubscriptionPlanId { get; set; }

    public SubscriptionStatus Status { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}