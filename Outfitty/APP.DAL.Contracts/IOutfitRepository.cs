using BASE.DAL.Contracts;
using Domain.Enums;

namespace APP.DAL.Contracts;

public interface IOutfitRepository : IBaseRepository<DTO.Outfit>, IOutfitRepositoryCustom
{
    
}

public interface IOutfitRepositoryCustom
{
    Task<IEnumerable<DTO.Outfit>> GetOutfitsByUserAsync(Guid userId);
    Task<IEnumerable<DTO.Outfit>> GetOutfitsBySeasonAsync(ClothingSeason season, Guid userId);
    Task<IEnumerable<DTO.Outfit>> GetOutfitsByStyleAsync(ClothingStyle style, Guid userId);
    Task<DTO.Outfit?> GetOutfitWithItemsAsync(Guid outfitId, Guid userId);
    Task<DTO.Outfit?> GetOutfitWithAllDetailsAsync(Guid outfitId, Guid userId);
    Task<IEnumerable<DTO.Outfit>> GetFavoriteOutfitsAsync(Guid userId);
    Task<int> GetOutfitsCountByUserAsync(Guid userId);
}