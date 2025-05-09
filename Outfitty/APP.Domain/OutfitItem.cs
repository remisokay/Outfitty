using BASE.Domain;

namespace Domain;

public class OutfitItem : BaseEntity
{
    //FK
    public Guid OutfitId { get; set; }
    public virtual Outfit? Outfit { get; set; }
    public int DisplayOrder { get; set; }
    
    public Guid ClothingItemId { get; set; }
    public virtual ClothingItem? ClothingItem { get; set; }
}