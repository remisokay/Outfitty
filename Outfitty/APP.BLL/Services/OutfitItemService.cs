using APP.BLL.Contracts;
using APP.BLL.DTO;
using APP.DAL.Contracts;
using BASE.BLL;
using BASE.Contracts;
using Domain.Enums;

namespace APP.BLL.Services;

public class OutfitItemService : BaseService<OutfitItem, APP.DAL.DTO.OutfitItem, IOutfitItemRepository>, IOutfitItemService
{
    private readonly IAppUow _uow;
    private readonly IMapper<Outfit, APP.DAL.DTO.Outfit> _outfitMapper;
    
    public OutfitItemService(
        IAppUow serviceUow,
        IMapper<OutfitItem, DAL.DTO.OutfitItem> mapper,
        IMapper<Outfit, APP.DAL.DTO.Outfit> outfitMapper)
        : base(serviceUow, serviceUow.OutfitItemRepository, mapper)
    {
        _uow = serviceUow;
        _outfitMapper = outfitMapper;
    }

    public override IEnumerable<OutfitItem> All(Guid userId = default)
    {
        var dalOutfitItems = ServiceRepository.All(userId);
        return dalOutfitItems.Select(oi => Mapper.Map(oi)!).ToList();
    }

    public override async Task<IEnumerable<OutfitItem>> AllAsync(Guid userId = default)
    {
        var dalOutfitItems = await ServiceRepository.AllAsync(userId);
        return dalOutfitItems.Select(oi => Mapper.Map(oi)!).ToList();
    }

    public override OutfitItem? Find(Guid id, Guid userId = default)
    {
        var dalOutfitItem = ServiceRepository.Find(id, userId);
        return dalOutfitItem != null ? Mapper.Map(dalOutfitItem)! : null;
    }

    public override async Task<OutfitItem?> FindAsync(Guid id, Guid userId = default)
    {
        var dalOutfitItem = await ServiceRepository.FindAsync(id, userId);
        return dalOutfitItem != null ? Mapper.Map(dalOutfitItem) : null;
    }

    public async Task<OutfitItem> AddItemToOutfitAsync(Guid outfitId, Guid clothingItemId, int displayOrder)
    {
        var dalOutfit = await _uow.OutfitRepository.FindAsync(outfitId);
        if (dalOutfit == null)
            throw new ArgumentException("Outfit not found");
        
        var dalClothingItem = await _uow.ClothingItemRepository.FindAsync(clothingItemId);
        if (dalClothingItem?.Wardrobe?.UserId != dalOutfit.UserId)
            throw new ArgumentException("Clothing item not found or does not belong to the same user");
        
        var existingOutfitItems = await ServiceRepository.GetOutfitItemsByOutfitAsync(outfitId, dalOutfit.UserId);
        if (existingOutfitItems.Any(oi => oi.ClothingItemId == clothingItemId))
            throw new InvalidOperationException("Clothing item is already in this outfit");
        
        // create new outfit item
        var newOutfitItem = new APP.DAL.DTO.OutfitItem
        {
            Id = Guid.NewGuid(),
            OutfitId = outfitId,
            ClothingItemId = clothingItemId,
            DisplayOrder = displayOrder
        };

        // add to repository
        ServiceRepository.Add(newOutfitItem);
        await _uow.SaveChangesAsync();

        var createdItem = await ServiceRepository.GetOutfitItemWithDetailsAsync(newOutfitItem.Id, dalOutfit.UserId);
        return Mapper.Map(createdItem)!;
    }

    public async Task RemoveItemFromOutfitAsync(Guid outfitId, Guid clothingItemId)
    {
        var dalOutfit = await _uow.OutfitRepository.FindAsync(outfitId);
        if (dalOutfit == null)
            throw new ArgumentException("Outfit not found");
        
        var outfitItems = await ServiceRepository.GetOutfitItemsByOutfitAsync(outfitId, dalOutfit.UserId);
        var outfitItemToRemove = outfitItems.FirstOrDefault(oi => oi.ClothingItemId == clothingItemId);
        
        if (outfitItemToRemove == null)
            throw new ArgumentException("Clothing item is not in this outfit");
        
        // remove
        await ServiceRepository.RemoveAsync(outfitItemToRemove.Id, dalOutfit.UserId);
        await _uow.SaveChangesAsync();
    }

    public async Task<IEnumerable<OutfitItem>> ReorderItemsAsync(Guid outfitId, IEnumerable<KeyValuePair<Guid, int>> itemOrderPairs)
    {
        var dalOutfit = await _uow.OutfitRepository.FindAsync(outfitId);
        if (dalOutfit == null)
            throw new ArgumentException("Outfit not found");
        
        var orderMap = itemOrderPairs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        // update display orders
        var success = await ServiceRepository.ReorderOutfitItemsAsync(outfitId, orderMap, dalOutfit.UserId);
        if (!success)
            throw new InvalidOperationException("Failed to reorder outfit items");
        
        await _uow.SaveChangesAsync();
        var updatedItems = await GetItemsByOutfitAsync(outfitId);
        return updatedItems.OrderBy(oi => oi.DisplayOrder);
    }

    public async Task<OutfitItem> UpdateDisplayOrderAsync(Guid outfitItemId, int newDisplayOrder)
    {
        var dalOutfitItem = await ServiceRepository.FindAsync(outfitItemId);
        if (dalOutfitItem == null)
            throw new ArgumentException("Outfit item not found");
        
        var dalOutfit = await _uow.OutfitRepository.FindAsync(dalOutfitItem.OutfitId);
        if (dalOutfit == null)
            throw new ArgumentException("Associated outfit not found");

        // update the display order
        dalOutfitItem.DisplayOrder = newDisplayOrder;
        await ServiceRepository.UpdateAsync(dalOutfitItem);
        await _uow.SaveChangesAsync();
        var updatedItem = await ServiceRepository.GetOutfitItemWithDetailsAsync(outfitItemId, dalOutfit.UserId);
        
        return Mapper.Map(updatedItem)!;
    }

    public async Task<IEnumerable<OutfitItem>> GetItemsByOutfitAsync(Guid outfitId)
    {
        var dalOutfit = await _uow.OutfitRepository.FindAsync(outfitId);
        if (dalOutfit == null)
            return Enumerable.Empty<OutfitItem>();

        var dalOutfitItems = await ServiceRepository.GetOutfitItemsByOutfitAsync(outfitId, dalOutfit.UserId);
        return dalOutfitItems
            .OrderBy(oi => oi.DisplayOrder)
            .Select(oi => Mapper.Map(oi)!)
            .ToList();
    }

    public async Task<IEnumerable<OutfitItem>> GetItemsByTypeInOutfitAsync(Guid outfitId, ClothingType type)
    {
        var allItems = await GetItemsByOutfitAsync(outfitId);
        return allItems.Where(oi => oi.ClothingItem?.Type == type).ToList();
    }

    public async Task<IEnumerable<OutfitItem>> GetOuterLayerItemsAsync(Guid outfitId)
    {
        var allItems = await GetItemsByOutfitAsync(outfitId);
        return allItems.Where(oi => 
                oi.ClothingItem?.Type == ClothingType.TopOuter || 
                oi.ClothingItem?.Type == ClothingType.BottomOuter).ToList();
    }

    public async Task<IEnumerable<OutfitItem>> GetInnerLayerItemsAsync(Guid outfitId)
    {
        var allItems = await GetItemsByOutfitAsync(outfitId);
        return allItems.Where(oi => 
                oi.ClothingItem?.Type == ClothingType.TopInner || 
                oi.ClothingItem?.Type == ClothingType.BottomInner)
            .ToList();
    }

    public async Task<IEnumerable<Outfit>> GetOutfitsContainingClothingItemAsync(Guid clothingItemId)
    {
        var dalClothingItem = await _uow.ClothingItemRepository.FindAsync(clothingItemId);
        if (dalClothingItem == null)
            return Enumerable.Empty<Outfit>();

        // outfit items that reference this clothing item
        var outfitItems = await ServiceRepository.GetOutfitItemsByClothingItemAsync(clothingItemId, dalClothingItem.Wardrobe?.UserId ?? Guid.Empty);
        var outfitIds = outfitItems.Select(oi => oi.OutfitId).Distinct();
        
        var outfits = new List<APP.DAL.DTO.Outfit>();
        foreach (var outfitId in outfitIds)
        {
            var outfit = await _uow.OutfitRepository.FindAsync(outfitId, dalClothingItem.Wardrobe?.UserId ?? Guid.Empty);
            if (outfit != null) outfits.Add(outfit);
            
        }
        
        return outfits.Select(o => _outfitMapper.Map(o)!).ToList();
    }
}