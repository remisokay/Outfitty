using System.ComponentModel.DataAnnotations;
using BASE.Domain.Identity;

namespace Domain.identity;

public class AppUser : BaseUser<AppUserRole>
{
    [MinLength(1)] 
    [MaxLength(128)]
    public string Username { get; set; } = default!;
    
    // TODO: FK Collections??
}