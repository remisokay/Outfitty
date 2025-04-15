using System.Collections;
using Microsoft.AspNetCore.Identity;

namespace BASE.Domain.Identity;

public abstract class BaseUser<TUserRole> : BaseUser<Guid, TUserRole>
    where TUserRole : class
{
}

public abstract class BaseUser<TKey, TUserRole> : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    where TUserRole : class
{
    public ICollection<TUserRole>? UserRoles { get; set; }
}