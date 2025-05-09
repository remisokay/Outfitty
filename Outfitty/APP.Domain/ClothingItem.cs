using System.ComponentModel.DataAnnotations;
using BASE.Domain;
using Domain.Enums;

namespace Domain;

public class ClothingItem : BaseEntity
{
    [MaxLength(128)]
    public string Name { get; set; } = default!; // h&m white plain t-shirt
    public ClothingType Type { get; set; }
    public ClothingStyle Style { get; set; }
    public ClothingSeason Season { get; set; }
    public ClothingColor PrimaryColor { get; set; }
    public ClothingColor? SecondaryColor { get; set; }
    
    //FK
    public Guid WardrobeId { get; set; }
    public virtual Wardrobe? Wardrobe { get; set; }
    // single item can be used in multiple outfits
    public virtual ICollection<OutfitItem>? OutfitItems { get; set; }
    
    public Guid ImageMetadataId { get; set; }
    public virtual ImageMetadata? ImageMetadata { get; set; }
}