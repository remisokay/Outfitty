using BASE.Contracts;
using Domain.Enums;
using Domain.identity;

namespace APP.BLL.DTO;

public class Favourite : IDomainId
{
    public Guid Id { get; set; }
    // [Display(Name = nameof(UserId), ResourceType = typeof(APP.Resources.Domain.Favourite))]
    public Guid UserId { get; set; }

    // [Display(Name = nameof(User), ResourceType = typeof(APP.Resources.Domain.Favourite))]
    public AppUser? User { get; set; }

    // [Display(Name = nameof(OutfitId), ResourceType = typeof(APP.Resources.Domain.Favourite))]
    public Guid OutfitId { get; set; }

    // [Display(Name = nameof(Outfit), ResourceType = typeof(APP.Resources.Domain.Favourite))]
    public Outfit? Outfit { get; set; }
    
    // BLL functions
    public string? OutfitName => Outfit?.Name;
    
    public ClothingStyle? OutfitStyle => Outfit?.Style;
    
    public ClothingSeason? OutfitSeason => Outfit?.Season;
    
    public int? OutfitItemCount => Outfit?.OutfitItems?.Count;
    
    private ClothingSeason GetCurrentSeason()
    {
        int month = DateTime.Now.Month;
        
        return month switch
        {
            >= 3 and <= 5 => ClothingSeason.Spring,
            >= 6 and <= 8 => ClothingSeason.Summer,
            >= 9 and <= 11 => ClothingSeason.Fall,
            _ => ClothingSeason.Winter
        };
    }
}