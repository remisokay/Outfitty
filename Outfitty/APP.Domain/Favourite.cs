using BASE.Domain;
using Domain.identity;

namespace Domain;

public class Favourite : BaseEntity
{
    //FK
    public Guid UserId { get; set; }
    public virtual AppUser? User { get; set; }
    
    public Guid OutfitId { get; set; }
    public virtual Outfit? Outfit { get; set; }
}