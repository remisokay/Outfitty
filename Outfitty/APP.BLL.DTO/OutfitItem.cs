using BASE.Contracts;
using Domain;
using Domain.Enums;

namespace APP.BLL.DTO;

public class OutfitItem: IDomainId
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
    
    // BLL functions
    public string LayerTypeDescription => ClothingItem?.Type switch
    {
        ClothingType.TopOuter => "Outer Top",
        ClothingType.TopInner => "Inner Top",
        ClothingType.BottomOuter => "Outer Bottom",
        ClothingType.BottomInner => "Inner Bottom",
        ClothingType.Footwear => "Footwear",
        ClothingType.Accessory => "Accessory",
        ClothingType.FullBody => "Full Body",
        _ => "Unknown"
    };
    
    public string ColorName => ClothingItem?.PrimaryColor.ToString() ?? "Unknown";
    
    public string StyleName => ClothingItem?.Style.ToString() ?? "Unknown";
    
    public string? ImageUrl => ClothingItem?.ImageMetadata?.PublicUrl;
    
    public int GetLayerPriority()
    {
        if (ClothingItem == null) return 999;
        
        return ClothingItem.Type switch
        {
            ClothingType.BottomInner => 1,
            ClothingType.TopInner => 2,
            ClothingType.BottomOuter => 3,
            ClothingType.TopOuter => 4,
            ClothingType.Footwear => 5,
            ClothingType.Accessory => 6,
            ClothingType.FullBody => 0,
            _ => 999
        };
    }
    
    public string GetVisualPosition()
    {
        if (ClothingItem == null) return "center";
        
        return ClothingItem.Type switch
        {
            ClothingType.TopOuter => "top",
            ClothingType.TopInner => "top",
            ClothingType.BottomOuter => "bottom",
            ClothingType.BottomInner => "bottom",
            ClothingType.Footwear => "bottom-center",
            ClothingType.Accessory => DisplayOrder % 2 == 0 ? "left" : "right",
            ClothingType.FullBody => "center",
            _ => "center"
        };
    }
    
}