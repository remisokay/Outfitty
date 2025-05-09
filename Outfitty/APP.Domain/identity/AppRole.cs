using BASE.Domain.Identity;

namespace Domain.identity;

public class AppRole : BaseRole<AppUserRole>
{
    public static class RoleNames
    {
        public const string Admin = "admin";
        public const string User = "user";
        // public const string PremiumUser = "premium"; -> for future?
    }
    
}