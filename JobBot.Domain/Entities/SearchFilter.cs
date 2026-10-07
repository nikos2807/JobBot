namespace JobBot.Domain.Entities;

public class SearchFilter
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string? Keywords { get; set; }

    public decimal? SalaryFrom { get; set; }

    public decimal? SalaryTo { get; set; }

    public string? Location { get; set; }

    public bool RemoteOnly { get; set; }

    public string? EmploymentType { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}