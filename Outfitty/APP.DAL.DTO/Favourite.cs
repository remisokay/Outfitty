namespace Domain;

public class Favourite : BaseEntity
{
    public string? Comment { get; set; }
    
    //FK
    public Guid UserId { get; set; }
    public User? User { get; set; }
    
    public ICollection<Outfit>? Outfits { get; set; } = default!;
}