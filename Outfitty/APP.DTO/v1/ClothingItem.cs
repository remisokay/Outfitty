using System.ComponentModel.DataAnnotations;
using BASE.Contracts;
using Domain.Enums;

namespace APP.DTO.v1;

public class ClothingItem : IDomainId
{
    public Guid Id { get; set; }
    
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
    
    // maybe extra
    public string? ImageUrl { get; set; }
    public string? WardrobeName { get; set; }
}