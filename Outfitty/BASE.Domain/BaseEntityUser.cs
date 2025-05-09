using BASE.Contracts;

namespace BASE.Domain;

public class BaseEntityUser<TUser> : BaseEntityUser<Guid, TUser>, IDomainId, IDomainUserId
    where TUser : class
{
    
}


public abstract class BaseEntityUser<TKey, TUser> : BaseEntity<TKey>, IDomainUserId<TKey>
    where TKey : IEquatable<TKey>
    where TUser : class
{
    public TKey UserId { get; set; } = default!;
    public TUser? User { get; set; }
}