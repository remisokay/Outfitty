using System.ComponentModel.DataAnnotations;

namespace APP.DTO.v1;

public class WardrobeCreate
{
    [Required]
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public Guid UserId { get; set; }
}