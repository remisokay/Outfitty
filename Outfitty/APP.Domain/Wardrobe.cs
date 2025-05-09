using System.ComponentModel.DataAnnotations;
using BASE.Domain;
using Domain.identity;

namespace Domain;

public class Wardrobe : BaseEntity
{
    [MaxLength(128)]
    public string Name { get; set; } = default!; // Summer wardrobe
    [MaxLength(500)]
    public string? Description { get; set; }
    
    // FK
    public Guid UserId { get; set; }
    public virtual AppUser? User { get; set; }
    public virtual ICollection<ClothingItem>? ClothingItems { get; set; }
    
}