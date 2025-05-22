using APP.BLL.Contracts;
using APP.BLL.DTO;
using APP.DAL.Contracts;
using BASE.BLL;
using BASE.Contracts;
using Domain.Enums;

namespace APP.BLL.Services;

public class ClothingItemService : BaseService<ClothingItem, APP.DAL.DTO.ClothingItem, IClothingItemRepository>, IClothingItemService
{
    private readonly IAppUow _uow;
    private readonly IMapper<Outfit, DAL.DTO.Outfit> _outfitMapper;


    public ClothingItemService(
        IAppUow serviceUow,
        IMapper<ClothingItem, DAL.DTO.ClothingItem> mapper,
        IMapper<Outfit, DAL.DTO.Outfit> outfitMapper)
        : base(serviceUow, serviceUow.ClothingItemRepository, mapper)
    {
        _uow = serviceUow;
        _outfitMapper = outfitMapper;
    }

    public async Task<IEnumerable<ClothingItem>> GetUserClothingItemsAsync(Guid userId)
    {
        var dalClothingItems = await ServiceRepository.AllAsync(userId);
        var bllClothingItems = dalClothingItems.Select(item => Mapper.Map(item)!).ToList();

        foreach (var item in bllClothingItems)
        {
            item.OutfitCount = await GetItemUsageCountAsync(item.Id);
        }
        
        return bllClothingItems;
        
    }

    public async Task<IEnumerable<Outfit>> GetOutfitsContainingItemAsync(Guid clothingItemId)
    {
        var clothingItem = await ServiceRepository.FindAsync(clothingItemId);
        if (clothingItem == null) return Enumerable.Empty<Outfit>();
        
        var outfitItems = await _uow.OutfitItemRepository.GetOutfitItemsByClothingItemAsync(clothingItemId, clothingItem.Wardrobe?.UserId ?? Guid.Empty);
        var outfitIds = outfitItems.Select(oi => oi.OutfitId).Distinct();
        
        // get the outfits
        var outfits = new List<APP.DAL.DTO.Outfit>();
        foreach (var outfitId in outfitIds)
        {
            var outfit = await _uow.OutfitRepository.FindAsync(outfitId, clothingItem.Wardrobe?.UserId ?? Guid.Empty);
            if (outfit != null) outfits.Add(outfit);
        }
        
        return outfits.Select(o => _outfitMapper.Map(o)!).ToList();
    }

    public async Task<ClothingItem> AssignImageToClothingItemAsync(Guid clothingItemId, Guid imageId)
    {
        var dalClothingItem = await ServiceRepository.FindAsync(clothingItemId);
        if (dalClothingItem == null)
            throw new ArgumentException($"Clothing item not found");
        
        var userId = dalClothingItem.Wardrobe?.UserId ?? Guid.Empty;
        if (userId == Guid.Empty)
            throw new InvalidOperationException($"User ID not determined for clothing item");
        
        var image = await _uow.ImageMetadataRepository.FindAsync(imageId, userId);
        if (image == null)
            throw new ArgumentException("Image not found in clothing item services");
        if (image.ClothingItemId.HasValue && image.ClothingItemId != clothingItemId)
            throw new InvalidOperationException("Image is already assigned to another clothing item");
        
        // assign image to clothing item
        image.ClothingItemId = clothingItemId;
        dalClothingItem.ImageMetadataId = imageId;
        // update
        await _uow.ImageMetadataRepository.UpdateAsync(image);
        await ServiceRepository.UpdateAsync(dalClothingItem);
        await _uow.SaveChangesAsync();
        // return
        var updated = await ServiceRepository.FindAsync(clothingItemId, userId);
        var bllItem = Mapper.Map(updated)!;
        bllItem.OutfitCount = await GetItemUsageCountAsync(clothingItemId);
        
        return bllItem;
    }

    public async Task<int> GetItemUsageCountAsync(Guid clothingItemId)
    {
        var clothingItem = await ServiceRepository.FindAsync(clothingItemId);
        if (clothingItem == null) return 0;
        
        var outfitItems = await _uow.OutfitItemRepository.GetOutfitItemsByClothingItemAsync(clothingItemId, clothingItem.Wardrobe?.UserId ?? Guid.Empty);
        return outfitItems.Select(oi => oi.OutfitId).Distinct().Count();
    }

    public async Task<IEnumerable<ClothingItem>> FilterItemsAsync(
        Guid userId,
        ClothingType? type = null,
        ClothingStyle? style = null,
        ClothingSeason? season = null,
        ClothingColor? color = null)
    {
        var all = await ServiceRepository.AllAsync(userId);
        
        var filtered = all.AsEnumerable();

        if (type.HasValue)
        {
            filtered = filtered.Where(item => item.Type == type.Value);
        }

        if (style.HasValue)
        {
            filtered = filtered.Where(item => item.Style == style.Value);
        }

        if (season.HasValue)
        {
            filtered = filtered.Where(item => item.Season == season.Value || item.Season == ClothingSeason.AllSeason);
        }

        if (color.HasValue)
        {
            filtered = filtered.Where(item =>
                item.PrimaryColor == color.Value ||
                (item.SecondaryColor.HasValue && item.SecondaryColor.Value == color.Value));
        }
        
        var bllItems = filtered.Select(item => Mapper.Map(item)!).ToList();

        foreach (var item in bllItems)
        {
            item.OutfitCount = await GetItemUsageCountAsync(item.Id);
        }

        return bllItems;
    }
    
    public async Task<IEnumerable<ClothingItem>> GetClothingItemsByWardrobeAsync(Guid wardrobeId, Guid userId)
    {
        var dalItems = await ServiceRepository.GetClothingItemsByWardrobeAsync(wardrobeId, userId);
        var bllItems = dalItems.Select(item => Mapper.Map(item)!).ToList();
        
        foreach (var item in bllItems)
        {
            item.OutfitCount = await GetItemUsageCountAsync(item.Id);
        }
        
        return bllItems;
    }
    
    public async Task<bool> DeleteClothingItemAsync(Guid clothingItemId, Guid userId)
    {
        var dalItem = await ServiceRepository.FindAsync(clothingItemId, userId);
        if (dalItem == null) return false;
        
        // check if the item is used in any outfits
        var usageCount = await GetItemUsageCountAsync(clothingItemId);
        if (usageCount > 0)
        {
            throw new InvalidOperationException($"Cannot delete clothing item as it is used in {usageCount} outfit(s). Remove it from all outfits first.");
        }
        
        // delete associated image if it exists
        if (dalItem.ImageMetadataId != Guid.Empty)
        {
            var image = await _uow.ImageMetadataRepository.FindAsync(dalItem.ImageMetadataId, userId);
            if (image != null)
            {
                await _uow.ImageMetadataRepository.DeleteImageAsync(dalItem.ImageMetadataId, userId);
            }
        }
        
        // delete
        await ServiceRepository.RemoveAsync(clothingItemId, userId);
        await _uow.SaveChangesAsync();
        
        return true;
    }
}