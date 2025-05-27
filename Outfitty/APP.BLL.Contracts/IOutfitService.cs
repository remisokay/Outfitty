using BASE.BLL.Contracts;
using Domain.Enums;

namespace APP.BLL.Contracts;

public interface IOutfitService : IBaseService<DTO.Outfit>
{
    Task<IEnumerable<DTO.Outfit>> GetUserOutfitsAsync(Guid userId);
    Task<DTO.Outfit?> GetOutfitWithItemsAsync(Guid outfitId, Guid userId);
    Task<DTO.Outfit> AssignImageToOutfitAsync(Guid outfitId, Guid imageId);
    
    Task<DTO.Outfit> AddClothingItemToOutfitAsync(Guid outfitId, Guid clothingItemId, int displayOrder);
    Task RemoveClothingItemFromOutfitAsync(Guid outfitId, Guid outfitItemId);
    Task ReorderOutfitItemsAsync(Guid outfitId, IEnumerable<KeyValuePair<Guid, int>> itemOrderPairs);
    
    // Filtering
    Task<IEnumerable<DTO.Outfit>> GetOutfitsBySeasonAsync(Guid userId, ClothingSeason season);
    Task<IEnumerable<DTO.Outfit>> GetOutfitsByStyleAsync(Guid userId, ClothingStyle style);
    Task<IEnumerable<DTO.Outfit>> GetFavoriteOutfitsAsync(Guid userId);
}