using BASE.Contracts;
using Microsoft.AspNetCore.Identity;

namespace BASE.Domain.Identity;

public abstract class BaseUserRole<TUser, TRole> : BaseUserRole<Guid, TUser, TRole> //, IDomainId
        where TUser : class
        where TRole : class
{
}

public abstract class BaseUserRole<TKey, TUser, TRole> : IdentityUserRole<TKey> //, IDomainId<TKey>
        where TKey : IEquatable<TKey>
        where TUser : class
        where TRole : class
{
        // causes problems with role manager
        // public TKey Id { get; set; } = default!;
        public TUser? User { get; set; }
        public TRole? Role { get; set; }
}