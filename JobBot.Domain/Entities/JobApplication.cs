using JobBot.Domain.Enums;

namespace JobBot.Domain.Entities;

public class JobApplication
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long VacancyId { get; set; }

    public ApplicationStatus Status { get; set; }

    public string? ExternalApplicationId { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}