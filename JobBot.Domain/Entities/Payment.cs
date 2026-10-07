namespace JobBot.Domain.Entities;

using JobBot.Domain.Enums;

public class Payment
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long SubscriptionPlanId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "RUB";

    public PaymentStatus Status { get; set; }

    public string? ExternalPaymentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? PaidAt { get; set; }
}