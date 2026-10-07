namespace JobBot.Domain.Entities;

public class Vacancy
{
    public long Id { get; set; }

    public long PlatformId { get; set; }

    public string ExternalId { get; set; } = null!;

    public string Title { get; set; } = null!;


    public string? Description { get; set; }

    public string? Url { get; set; }

    public decimal? SalaryFrom { get; set; }

    public decimal? SalaryTo { get; set; }

    public string? Currency { get; set; }

    public string? Location { get; set; }

    public bool Remote { get; set; }

    public DateTime PublishedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}