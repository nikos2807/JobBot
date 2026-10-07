namespace JobBot.Domain.Entities;

public class SubscriptionPlan
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int DurationDays { get; set; }

    public int MaxApplicationsPerDay { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}