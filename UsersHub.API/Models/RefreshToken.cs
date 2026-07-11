using UsersHub.API.Models;

namespace UsersHub.API.Models;

public class RefreshToken
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public ApplicationUser User { get; set; } = default!;

    public bool IsExpired => ExpiresAt <= DateTime.UtcNow;

    public bool IsRevoked => RevokedAt != null;
}