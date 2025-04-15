using BASE.Contracts;
using BASE.Domain.Identity;


namespace Domain;

public class AppRefreshToken : BaseRefreshToken, IDomainUserId
{
    public Guid Type { get; set; }
    public Guid UserId { get; set; }
}