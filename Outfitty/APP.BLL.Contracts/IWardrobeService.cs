using BASE.BLL.Contracts;
using Domain.Enums;

namespace APP.BLL.Contracts;

public interface IWardrobeService : IBaseService<DTO.Wardrobe>
{
    Task<IEnumerable<DTO.Wardrobe>> GetUserWardrobesAsync(Guid userId);
    Task<DTO.Wardrobe?> GetWardrobeWithItemsAsync(Guid wardrobeId, Guid userId);
    Task<DTO.ClothingItem> AddClothingItemToWardrobeAsync(Guid wardrobeId, DTO.ClothingItem clothingItem);
    Task RemoveClothingItemFromWardrobeAsync(Guid wardrobeId, Guid clothingItemId);

    Task<Dictionary<ClothingType, int>> GetWardrobeStatisticsAsync(Guid wardrobeId);
    
    // Filtering
    Task<IEnumerable<DTO.ClothingItem>> GetItemsByTypeAsync(Guid wardrobeId, ClothingType type);
    Task<IEnumerable<DTO.ClothingItem>> GetItemsBySeasonAsync(Guid wardrobeId, ClothingSeason season);
    Task<IEnumerable<DTO.ClothingItem>> GetItemsByStyleAsync(Guid wardrobeId, ClothingStyle style);
    Task<IEnumerable<DTO.ClothingItem>> GetItemsByColorAsync(Guid wardrobeId, ClothingColor color);
    
}