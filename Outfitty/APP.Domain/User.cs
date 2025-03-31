using System.ComponentModel.DataAnnotations;

namespace Domain;

public class User : BaseEntity
{
    [MaxLength(128)]
    public string Username { get; set; } = default!;
    [MaxLength(128)]
    public string Password { get; set; } = default!;
    [MaxLength(254)]
    public string Email { get; set; } = default!;
    [MaxLength(2000)]
    public string? Photo { get; set; } = default!; // url or path
    
    public ICollection<ClothesItem>? ClothesItems { get; set; }
    public ICollection<Wardrobe>? Wardrobes { get; set; }
    public ICollection<Outfit>? Outfits { get; set; }
    public ICollection<Favourite>? Favourites { get; set; }
}