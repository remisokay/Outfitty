using BASE.Contracts;

namespace BASE.Domain;

public class BaseEntity : BaseEntity<Guid>, IDomainId
{
    
}

public abstract class BaseEntity<TKey> : IDomainId<TKey>
    where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; } = default!;
}