using BASE.Contracts;

namespace WebApp.Helpers;

public class UsernameResolver : IUsernameResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public UsernameResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public string CurrentUserName => _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "system";
}