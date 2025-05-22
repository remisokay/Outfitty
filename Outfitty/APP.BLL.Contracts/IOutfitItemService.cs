using BASE.BLL.Contracts;
using Domain.Enums;

namespace APP.BLL.Contracts;

public interface IOutfitItemService : IBaseService<DTO.OutfitItem>
{
    Task<DTO.OutfitItem> AddItemToOutfitAsync(Guid outfitId, Guid clothingItemId, int displayOrder);
    Task RemoveItemFromOutfitAsync(Guid outfitId, Guid clothingItemId);
    
    // Reordering items within an outfit
    Task<IEnumerable<DTO.OutfitItem>> ReorderItemsAsync(Guid outfitId, IEnumerable<KeyValuePair<Guid, int>> itemOrderPairs);
    Task<DTO.OutfitItem> UpdateDisplayOrderAsync(Guid outfitItemId, int newDisplayOrder);
    
    // Filtering and categorization
    Task<IEnumerable<DTO.OutfitItem>> GetItemsByTypeInOutfitAsync(Guid outfitId, ClothingType type);
    Task<IEnumerable<DTO.OutfitItem>> GetOuterLayerItemsAsync(Guid outfitId);
    Task<IEnumerable<DTO.OutfitItem>> GetInnerLayerItemsAsync(Guid outfitId);
    
    // Find outfits where a specific clothing item is used
    Task<IEnumerable<DTO.Outfit>> GetOutfitsContainingClothingItemAsync(Guid clothingItemId);
}