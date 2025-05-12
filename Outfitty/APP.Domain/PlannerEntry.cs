using System.ComponentModel.DataAnnotations;
using BASE.Domain;
using Domain.identity;

namespace Domain;

public class PlannerEntry : BaseEntity
{
    public DateTime Date { get; set; }
    [MaxLength(128)]
    public string Title { get; set; } = default!;
    [MaxLength(500)]
    public string? Comment { get; set; }
    
    public TimeSpan? Time { get; set; } // allows multiple entries on the same day
    
    //FK
    public Guid UserId { get; set; }
    public virtual AppUser? User { get; set; }
    
    public Guid OutfitId { get; set; }
    public virtual Outfit? Outfit { get; set; }
    
}