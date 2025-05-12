using System.ComponentModel.DataAnnotations;
using BASE.Contracts;
using Domain;

namespace APP.DAL.DTO;

public class OutfitItem : IDomainId
{
    public Guid Id { get; set; }
    
    // [Display(Name = nameof(OutfitId), ResourceType = typeof(APP.Resources.Domain.OutfitItem))]
    public Guid OutfitId { get; set; }

    // [Display(Name = nameof(Outfit), ResourceType = typeof(APP.Resources.Domain.OutfitItem))]
    public Outfit? Outfit { get; set; }

    // [Display(Name = nameof(DisplayOrder), ResourceType = typeof(APP.Resources.Domain.OutfitItem))]
    public int DisplayOrder { get; set; }

    // [Display(Name = nameof(ClothingItemId), ResourceType = typeof(APP.Resources.Domain.OutfitItem))]
    public Guid ClothingItemId { get; set; }

    // [Display(Name = nameof(ClothingItem), ResourceType = typeof(APP.Resources.Domain.OutfitItem))]
    public ClothingItem? ClothingItem { get; set; }
    
}