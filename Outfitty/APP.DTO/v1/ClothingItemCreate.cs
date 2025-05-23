using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace APP.DTO.v1;

public class ClothingItemCreate
{
    [Required]
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    
    public ClothingType Type { get; set; }
    public ClothingStyle Style { get; set; }
    public ClothingSeason Season { get; set; }
    public ClothingColor PrimaryColor { get; set; }
    public ClothingColor? SecondaryColor { get; set; }
    
    public Guid WardrobeId { get; set; }
    public Guid ImageMetadataId { get; set; }
}