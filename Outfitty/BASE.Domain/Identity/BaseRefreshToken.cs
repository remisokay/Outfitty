namespace BASE.Domain.Identity;
// handling JWT (JSON Web Token)
public class BaseRefreshToken : BaseRefreshToken<Guid>
{
    
}

public class BaseRefreshToken<TKey> : BaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    public string RefreshToken { get; set; } = Guid.NewGuid().ToString();
    public DateTime Expiration { get; set; } = DateTime.UtcNow.AddDays(7);
    // allows a short grace period where the previous token is still valid
    public string? PreviousRefreshToken { get; set; }
    public DateTime PreviousExpiration { get; set; } = DateTime.UtcNow.AddDays(7);
}