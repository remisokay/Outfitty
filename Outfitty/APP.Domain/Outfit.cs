using System.ComponentModel.DataAnnotations;
using BASE.Domain;
using Domain.Enums;
using Domain.identity;

namespace Domain;

public class Outfit : BaseEntity
{
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public ClothingSeason Season { get; set; }
    public ClothingStyle Style { get; set; }
    
    //FK
    public Guid UserId { get; set; }
    public virtual AppUser? User { get; set; }
    
    public virtual ICollection<OutfitItem>? OutfitItems { get; set; }
    public virtual ICollection<PlannerEntry>? PlannerEntries { get; set; }
    public virtual ICollection<Favourite>? Favourites { get; set; }
    
    
    
}