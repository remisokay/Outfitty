using BASE.DAL.Contracts;

namespace APP.DAL.Contracts;

public interface IWardrobeRepository : IBaseRepository<DTO.Wardrobe>, IWardrobeRepositoryCustom
{
    
}

public interface IWardrobeRepositoryCustom
{
    Task<IEnumerable<DTO.Wardrobe>> GetWardrobesByUserAsync(Guid userId);
    Task<DTO.Wardrobe?> GetWardrobeWithClothingItemsAsync(Guid wardrobeId, Guid userId);
    Task<int> GetClothingItemsCountAsync(Guid wardrobeId, Guid userId);
}