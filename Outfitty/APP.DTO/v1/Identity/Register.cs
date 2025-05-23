using System.ComponentModel.DataAnnotations;

namespace APP.DTO.v1.Identity;

public class Register
{
    [MaxLength(128)]
    public string Email { get; set; } = default!;
    
    [MinLength(1)]
    [MaxLength(128)]
    public string Username { get; set; }= default!;

    [MaxLength(128)]
    public string Password { get; set; } = default!;
    
    public string? Bio { get; set; }

    
}