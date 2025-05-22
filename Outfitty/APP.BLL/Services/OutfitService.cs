using APP.BLL.Contracts;
using APP.BLL.DTO;
using APP.DAL.Contracts;
using BASE.BLL;
using BASE.Contracts;
using Domain.Enums;

namespace APP.BLL.Services;

public class OutfitService : BaseService<Outfit, APP.DAL.DTO.Outfit, IOutfitRepository>, IOutfitService
{
    private readonly IAppUow _uow;
    // private readonly IMapper<OutfitItem, APP.DAL.DTO.OutfitItem> _outfitItemMapper;
    // private readonly IMapper<ClothingItem, APP.DAL.DTO.ClothingItem> _clothingItemMapper;
    
    public OutfitService(
        IAppUow serviceUow,
        IMapper<Outfit, DAL.DTO.Outfit> mapper
        // IMapper<OutfitItem, APP.DAL.DTO.OutfitItem> outfitItemMapper,
        // IMapper<ClothingItem, APP.DAL.DTO.ClothingItem> clothingItemMapper
        )
        : base(serviceUow, serviceUow.OutfitRepository, mapper)
    {
        _uow = serviceUow;
        // _outfitItemMapper = outfitItemMapper;
        // _clothingItemMapper = clothingItemMapper;
    }

    public async Task<IEnumerable<Outfit>> GetUserOutfitsAsync(Guid userId)
    {
        var dalOutfits = await ServiceRepository.GetOutfitsByUserAsync(userId);
        var bllOutfits = dalOutfits.Select(outfit => Mapper.Map(outfit)!).ToList();
        
        foreach (var outfit in bllOutfits)
        {
            // favourite check
            outfit.IsFavourite = await _uow.FavouriteRepository.IsFavouriteAsync(outfit.Id, userId);
            // TODO: more business calculating 
        }
        
        return bllOutfits;
    }

    public async Task<Outfit?> GetOutfitWithItemsAsync(Guid outfitId, Guid userId)
    {
        var dalOutfit = await ServiceRepository.GetOutfitWithAllDetailsAsync(outfitId, userId);
        if (dalOutfit == null) return null;
        
        var bllOutfit = Mapper.Map(dalOutfit);
        if (bllOutfit == null) return null;
        
        // set favorite status
        bllOutfit.IsFavourite = await _uow.FavouriteRepository.IsFavouriteAsync(outfitId, userId);
        
        // TODO: business logic calculating
        
        return bllOutfit;
    }

    public async Task<Outfit> AddClothingItemToOutfitAsync(Guid outfitId, Guid clothingItemId, int displayOrder)
    {
        var dalOutfit = await ServiceRepository.FindAsync(outfitId);
        if (dalOutfit == null)
            throw new ArgumentException("Outfit not found");
        
        var dalClothingItem = await _uow.ClothingItemRepository.FindAsync(clothingItemId);
        if (dalClothingItem?.Wardrobe?.UserId != dalOutfit.UserId)
            throw new ArgumentException("Clothing item not found or does not belong to the same user");
        
        var existingOutfitItems = await _uow.OutfitItemRepository.GetOutfitItemsByOutfitAsync(outfitId, dalOutfit.UserId);
        if (existingOutfitItems.Any(oi => oi.ClothingItemId == clothingItemId))
            throw new InvalidOperationException("Clothing item is already in this outfit");
        
        var newOutfitItem = new APP.DAL.DTO.OutfitItem
        {
            Id = Guid.NewGuid(),
            OutfitId = outfitId,
            ClothingItemId = clothingItemId,
            DisplayOrder = displayOrder
        };
        
        // add to repository
        _uow.OutfitItemRepository.Add(newOutfitItem);
        await _uow.SaveChangesAsync();
        
        return await GetOutfitWithItemsAsync(outfitId, dalOutfit.UserId) 
               ?? throw new InvalidOperationException("Failed to retrieve updated outfit");
    }

    public async Task RemoveClothingItemFromOutfitAsync(Guid outfitId, Guid outfitItemId)
    {
        var dalOutfit = await ServiceRepository.FindAsync(outfitId);
        if (dalOutfit == null)
            throw new ArgumentException("Outfit not found");
        
        var dalOutfitItem = await _uow.OutfitItemRepository.FindAsync(outfitItemId, dalOutfit.UserId);
        if (dalOutfitItem?.OutfitId != outfitId)
            throw new ArgumentException("Outfit item not found or does not belong to this outfit");
        
        // remove the outfit item
        await _uow.OutfitItemRepository.RemoveAsync(outfitItemId, dalOutfit.UserId);
        await _uow.SaveChangesAsync();
    }

    public async Task ReorderOutfitItemsAsync(Guid outfitId, IEnumerable<KeyValuePair<Guid, int>> itemOrderPairs)
    {
        var dalOutfit = await ServiceRepository.FindAsync(outfitId);
        if (dalOutfit == null)
            throw new ArgumentException("Outfit not found");
        
        var orderMap = itemOrderPairs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        
        // update order
        var success = await _uow.OutfitItemRepository.ReorderOutfitItemsAsync(outfitId, orderMap, dalOutfit.UserId);
        if (!success)
            throw new InvalidOperationException("Failed to reorder outfit items");
        
        await _uow.SaveChangesAsync();
    }

    public async Task<IEnumerable<Outfit>> GetOutfitsBySeasonAsync(Guid userId, ClothingSeason season)
    {
        var dalOutfits = await ServiceRepository.GetOutfitsBySeasonAsync(season, userId);
        var bllOutfits = dalOutfits.Select(outfit => Mapper.Map(outfit)!).ToList();
        
        return bllOutfits;
    }

    public async Task<IEnumerable<Outfit>> GetOutfitsByStyleAsync(Guid userId, ClothingStyle style)
    {
        var dalOutfits = await ServiceRepository.GetOutfitsByStyleAsync(style, userId);
        var bllOutfits = dalOutfits.Select(outfit => Mapper.Map(outfit)!).ToList();
        
        return bllOutfits;
    }

    public async Task<IEnumerable<Outfit>> GetFavoriteOutfitsAsync(Guid userId)
    {
        var dalOutfits = await ServiceRepository.GetFavoriteOutfitsAsync(userId);
        var bllOutfits = dalOutfits.Select(outfit => Mapper.Map(outfit)!).ToList();
        
        foreach (var outfit in bllOutfits)
        {
            outfit.IsFavourite = true;
        }
        
        return bllOutfits;
    }
    
    public async Task<bool> DeleteOutfitAsync(Guid outfitId, Guid userId)
    {
        var dalOutfit = await ServiceRepository.FindAsync(outfitId, userId);
        if (dalOutfit == null) return false;
        
        // remove all outfit items
        var outfitItems = await _uow.OutfitItemRepository.GetOutfitItemsByOutfitAsync(outfitId, userId);
        foreach (var item in outfitItems)
        {
            await _uow.OutfitItemRepository.RemoveAsync(item.Id, userId);
        }
        
        // remove from favorites
        var favorites = await _uow.FavouriteRepository.GetFavouritesByUserAsync(userId);
        var outfitFavorites = favorites.Where(f => f.OutfitId == outfitId);
        foreach (var favorite in outfitFavorites)
        {
            await _uow.FavouriteRepository.RemoveAsync(favorite.Id, userId);
        }
        
        // remove planner entries
        var plannerEntries = await _uow.PlannerEntryRepository.GetPlannerEntriesByUserAsync(userId);
        var outfitPlannerEntries = plannerEntries.Where(pe => pe.OutfitId == outfitId);
        foreach (var entry in outfitPlannerEntries)
        {
            await _uow.PlannerEntryRepository.RemoveAsync(entry.Id, userId);
        }
        
        // final remove
        await ServiceRepository.RemoveAsync(outfitId, userId);
        await _uow.SaveChangesAsync();
        
        return true;
    }
}