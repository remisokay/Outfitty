using System.ComponentModel.DataAnnotations;
using BASE.Contracts;

namespace BASE.Domain;

public class BaseEntity : BaseEntity<Guid>, IDomainId
{
    // uses guid as the type of id
}

public abstract class BaseEntity<TKey> : IDomainId<TKey>, IDomainMeta
    where TKey : IEquatable<TKey>
{
    // works with different id types
    public TKey Id { get; set; } = default!;
    
    [MaxLength(64)]
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    [MaxLength(64)]
    public string? ChangedBy { get; set; } = default!;
    public DateTime? ChangedAt { get; set; }
    
    
    public string? SysNotes { get; set; }
}