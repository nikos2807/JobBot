using JobBot.Domain.Enums;

namespace JobBot.Domain.Entities;

public class Platform
{
    public long Id { get; set; }

    public PlatformType Type { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}