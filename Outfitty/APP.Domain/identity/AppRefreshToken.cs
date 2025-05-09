using BASE.Contracts;
using BASE.Domain.Identity;

namespace Domain.identity;

public class AppRefreshToken : BaseRefreshToken, IDomainUserId
{
    public Guid UserId { get; set; }
    public AppUser? User { get; set; }
}