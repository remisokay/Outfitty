using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Planner : BaseEntity
{
    public DateTime Day { get; set; } = default!;
    [MaxLength(254)]
    public string? Comment { get; set; }
    
    
    //FK
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public ICollection<Outfit> Outfits { get; set; } = default!;
    
}