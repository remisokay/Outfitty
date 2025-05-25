using BASE.BLL.Contracts;

namespace APP.BLL.Contracts;

public interface IFavouriteService : IBaseService<DTO.Favourite>
{
    Task<IEnumerable<DTO.Favourite>> GetUserFavoritesAsync(Guid userId);
    Task<bool> ToggleFavoriteAsync(Guid userId, Guid outfitId); // Returns true if favorite, false if not
    Task<bool> IsOutfitFavoriteAsync(Guid userId, Guid outfitId);
    Task<DTO.Favourite> AddToFavoritesAsync(Guid userId, Guid outfitId);
    Task<bool> RemoveFromFavoritesAsync(Guid userId, Guid outfitId);
}