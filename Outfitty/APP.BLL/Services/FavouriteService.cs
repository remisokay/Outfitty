using APP.BLL.Contracts;
using APP.BLL.DTO;
using APP.DAL.Contracts;
using BASE.BLL;
using BASE.Contracts;

namespace APP.BLL.Services;

public class FavouriteService : BaseService<Favourite, DAL.DTO.Favourite, IFavouriteRepository>, IFavouriteService
{
    private readonly IAppUow _uow;
    private readonly IMapper<Outfit, DAL.DTO.Outfit> _outfitMapper;

    public FavouriteService(
        IAppUow serviceUow,
        IMapper<Favourite, DAL.DTO.Favourite> mapper,
        IMapper<Outfit, DAL.DTO.Outfit> outfitMapper)
        : base(serviceUow, serviceUow.FavouriteRepository, mapper)
    {
        _uow = serviceUow;
        _outfitMapper = outfitMapper;
    }

    public async Task<IEnumerable<Favourite>> GetUserFavoritesAsync(Guid userId)
    {
        var dalFavorites = await ServiceRepository.GetFavouritesByUserAsync(userId);
        var bllFavorites = dalFavorites.Select(fav => Mapper.Map(fav)!).ToList();

        foreach (var favorite in bllFavorites)
            await GetDetails(favorite, userId);
        
        return bllFavorites.ToList();
    }

    private async Task GetDetails(Favourite favorite, Guid userId)
    {
        if (favorite.Outfit == null) return;
        
        var detailedOutfit = await _uow.OutfitRepository.GetOutfitWithItemsAsync(favorite.OutfitId, userId);
        if (detailedOutfit != null)
        {
            var bllDetailedOutfit = _outfitMapper.Map(detailedOutfit)!;
            
            favorite.Outfit = bllDetailedOutfit;
            favorite.Outfit.IsFavourite = true;
        }
    }

    public async Task<bool> ToggleFavoriteAsync(Guid userId, Guid outfitId)
    {
        var dalOutfit = await _uow.OutfitRepository.FindAsync(outfitId, userId);
        if (dalOutfit == null)
            throw new ArgumentException("Outfit not found or does not belong to the user");
        
        var isNowFavorite = await ServiceRepository.ToggleFavouriteAsync(outfitId, userId);
        return isNowFavorite;
    }

    public async Task<bool> IsOutfitFavoriteAsync(Guid userId, Guid outfitId)
    { 
        return await ServiceRepository.IsFavouriteAsync(outfitId, userId);
    }
    
    public async Task<Favourite> AddToFavoritesAsync(Guid userId, Guid outfitId)
    {
        var dalOutfit = await _uow.OutfitRepository.FindAsync(outfitId, userId);
        if (dalOutfit == null) 
            throw new ArgumentException("Outfit not found or does not belong to the user");
        
        var existingFavorite = await ServiceRepository.GetFavouriteByOutfitAndUserAsync(outfitId, userId);
        if (existingFavorite != null)
            throw new InvalidOperationException("Outfit is already in favorites");
        
        var newFavorite = new APP.DAL.DTO.Favourite
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OutfitId = outfitId
        };

        ServiceRepository.Add(newFavorite);
        await _uow.SaveChangesAsync();

        var createdFavorite = await ServiceRepository.FindAsync(newFavorite.Id, userId);
        var bllFavorite = Mapper.Map(createdFavorite)!;
        await GetDetails(bllFavorite, userId);

        return bllFavorite;
    }
    
    public async Task<bool> RemoveFromFavoritesAsync(Guid userId, Guid outfitId)
    {
        var existingFavorite = await ServiceRepository.GetFavouriteByOutfitAndUserAsync(outfitId, userId);
        if (existingFavorite == null) return false;

        await ServiceRepository.RemoveAsync(existingFavorite.Id, userId);
        await _uow.SaveChangesAsync();

        return true;
    }
}