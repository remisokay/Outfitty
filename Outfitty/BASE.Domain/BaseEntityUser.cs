using BASE.Contracts;

namespace BASE.Domain;

public class BaseEntityUser<TUser, TRole> : BaseEntityUser<Guid, TUser, TRole>, IDomainId, IDomainUserId
    where TUser : class
    where TRole : class
{
    
}


public abstract class BaseEntityUser<TKey, TUser, TRole> : IDomainId<TKey>, IDomainUserId<TKey>
    where TKey : IEquatable<TKey>
    where TUser : class
    where TRole : class
{
    public TKey Id { get; set; } = default!;
    public TKey UserId { get; set; } = default!;
    public TUser? User { get; set; }
}