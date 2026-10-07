namespace JobBot.Domain.Entities;

public class PlatformAccount
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long PlatformId { get; set; }

    public string? ExternalUserId { get; set; }

    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? TokenExpiresAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}