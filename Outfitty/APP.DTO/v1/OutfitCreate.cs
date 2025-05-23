using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace APP.DTO.v1;

public class OutfitCreate
{
    [Required]
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public ClothingSeason Season { get; set; }
    public ClothingStyle Style { get; set; }
    
    public Guid UserId { get; set; }
}