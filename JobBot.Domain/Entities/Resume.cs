namespace JobBot.Domain.Entities;

public class Resume
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string FileName { get; set; } = null!;

    public string? FileUrl { get; set; }

    public string? Text { get; set; }

    public string? ParsedData { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}