using BASE.BLL.Contracts;
using Domain.Enums;

namespace APP.BLL.Contracts;

public interface IClothingItemService : IBaseService<DTO.ClothingItem>
{
    Task<IEnumerable<DTO.ClothingItem>> GetUserClothingItemsAsync(Guid userId);
    Task<IEnumerable<DTO.Outfit>> GetOutfitsContainingItemAsync(Guid clothingItemId);

    Task<IEnumerable<DTO.ClothingItem>> GetClothingItemsByWardrobeAsync(Guid wardrobeId, Guid userId);

    Task<bool> DeleteClothingItemAsync(Guid clothingItemId, Guid userId);
    
    Task<DTO.ClothingItem> AssignImageToClothingItemAsync(Guid clothingItemId, Guid imageId);
    Task<int> GetItemUsageCountAsync(Guid clothingItemId);
    
    // Filtering
    Task<IEnumerable<DTO.ClothingItem>> FilterItemsAsync(
        Guid userId, 
        ClothingType? type = null, 
        ClothingStyle? style = null, 
        ClothingSeason? season = null, 
        ClothingColor? color = null
        );
}