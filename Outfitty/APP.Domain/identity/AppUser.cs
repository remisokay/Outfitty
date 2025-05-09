using System.ComponentModel.DataAnnotations;
using BASE.Domain.Identity;

namespace Domain.identity;

public class AppUser : BaseUser<AppUserRole>
{
    [MinLength(1)] 
    [MaxLength(128)]
    public string Username { get; set; } = default!;
    
    // Profile info
    public Guid? ProfileImageId { get; set; }
    public virtual ImageMetadata? ProfileImage { get; set; }
    
    [MaxLength(1024)]
    public string? Bio { get; set; }
    
    // Stats
    public int TotalOutfits { get; set; } = 0;
    public int TotalClothingItems { get; set; } = 0;
    
    // FK
    public virtual ICollection<Wardrobe>? Wardrobes { get; set; }
    public virtual ICollection<Outfit>? Outfits { get; set; }
    public virtual ICollection<Favourite>? Favourites { get; set; }
    public virtual ICollection<PlannerEntry>? PlannerEntries { get; set; }
    public virtual ICollection<AppRefreshToken>? RefreshTokens { get; set; }
}