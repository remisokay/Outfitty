using BASE.DAL.Contracts;

namespace APP.DAL.Contracts;

public interface IFavouriteRepository : IBaseRepository<DTO.Favourite>, IFavouriteRepositoryCustom
{
    
}

public interface IFavouriteRepositoryCustom
{
    Task<IEnumerable<DTO.Favourite>> GetFavouritesByUserAsync(Guid userId);
    Task<bool> IsFavouriteAsync(Guid outfitId, Guid userId);
    Task<DTO.Favourite?> GetFavouriteByOutfitAndUserAsync(Guid outfitId, Guid userId);
    Task<bool> ToggleFavouriteAsync(Guid outfitId, Guid userId);
}