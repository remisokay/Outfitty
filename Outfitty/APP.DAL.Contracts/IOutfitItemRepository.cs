using BASE.DAL.Contracts;

namespace APP.DAL.Contracts;

public interface IOutfitItemRepository : IBaseRepository<DTO.OutfitItem>, IOutfitItemRepositoryCustom
{
    
}

public interface IOutfitItemRepositoryCustom
{
    Task<IEnumerable<DTO.OutfitItem>> GetOutfitItemsByOutfitAsync(Guid outfitId, Guid userId);
    Task<IEnumerable<DTO.OutfitItem>> GetOutfitItemsByClothingItemAsync(Guid clothingItemId, Guid userId);
    Task<DTO.OutfitItem?> GetOutfitItemWithDetailsAsync(Guid outfitItemId, Guid userId);
    Task<bool> ReorderOutfitItemsAsync(Guid outfitId, Dictionary<Guid, int> itemOrderMap, Guid userId);
}