using JobBot.Domain.Enums;

namespace JobBot.Domain.Entities;

public class User
{
    public long Id { get; set; }

    public long TelegramId { get; set; }

    public string? Username { get; set; }

    public string? FirstName { get; set; }
    public UserStatus Status { get; set; }

    public string? LastName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}