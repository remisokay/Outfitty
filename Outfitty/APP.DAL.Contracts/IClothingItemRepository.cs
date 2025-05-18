using BASE.DAL.Contracts;
using Domain.Enums;

namespace APP.DAL.Contracts;

public interface IClothingItemRepository : IBaseRepository<DTO.ClothingItem>, IClothingItemRepositoryCustom
{
    
}

public interface IClothingItemRepositoryCustom
{
    Task<IEnumerable<DTO.ClothingItem>> GetClothingItemsByWardrobeAsync(Guid wardrobeId, Guid userId);
    Task<IEnumerable<DTO.ClothingItem>> GetClothingItemsByTypeAsync(ClothingType type, Guid userId);
    Task<IEnumerable<DTO.ClothingItem>> GetClothingItemsBySeasonAsync(ClothingSeason season, Guid userId);
    Task<IEnumerable<DTO.ClothingItem>> GetClothingItemsByStyleAsync(ClothingStyle style, Guid userId);
    Task<IEnumerable<DTO.ClothingItem>> GetClothingItemsByColorAsync(ClothingColor color, Guid userId);
    Task<DTO.ClothingItem?> GetClothingItemWithImageAsync(Guid clothingItemId, Guid userId);
    Task<int> GetClothingItemsCountByUserAsync(Guid userId);
}