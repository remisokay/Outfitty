using System.ComponentModel.DataAnnotations;

namespace Domain;

public class ClothesItem : BaseEntity
{
    [MaxLength(128)]
    public string Name { get; set; } = default!; // h&m white plain t-shirt
    [MaxLength(2000)]
    public string Photo { get; set; } = default!;
    
    //FK
    public Guid UserId { get; set; }
    public User? User { get; set; }
    
    public Guid WardrobeId { get; set; }
    public Wardrobe? Wardrobe { get; set; }
    
    public Guid ClothesTypeId { get; set; }
    public ClothesItemType? ClothesType { get; set; } = default!;
}