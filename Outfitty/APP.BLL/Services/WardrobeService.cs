using APP.BLL.Contracts;
using APP.BLL.DTO;
using APP.DAL.Contracts;
using BASE.BLL;
using BASE.Contracts;
using Domain.Enums;

namespace APP.BLL.Services;

public class WardrobeService : BaseService<Wardrobe, APP.DAL.DTO.Wardrobe, IWardrobeRepository>, IWardrobeService
{
    private readonly IMapper<ClothingItem, APP.DAL.DTO.ClothingItem> _clothingItemMapper;
    private readonly IAppUow _uow;
    
    public WardrobeService(
        IAppUow serviceUow,
        IMapper<Wardrobe, APP.DAL.DTO.Wardrobe> mapper,
        IMapper<ClothingItem, APP.DAL.DTO.ClothingItem> clothingItemMapper)
        : base(serviceUow, serviceUow.WardrobeRepository, mapper)
    {
        _clothingItemMapper = clothingItemMapper;
        _uow = serviceUow;
    }

    public async Task<IEnumerable<Wardrobe>> GetUserWardrobesAsync(Guid userId)
    {
        var dalWardrobes = await ServiceRepository.GetWardrobesByUserAsync(userId);
        var bllWardrobes = dalWardrobes.Select(w => Mapper.Map(w)!).ToList();
        // count of clothing items for each wardrobe
        foreach (var wardrobe in bllWardrobes)
        {
            var count = await ServiceRepository.GetClothingItemsCountAsync(wardrobe.Id, userId);
            wardrobe.ItemCount = count;
        }
        
        return bllWardrobes;
    }

    public async Task<Wardrobe?> GetWardrobeWithItemsAsync(Guid wardrobeId, Guid userId)
    {
        var dalWardrobe = await ServiceRepository.GetWardrobeWithClothingItemsAsync(wardrobeId, userId);
        if (dalWardrobe == null) return null;
        
        var bllWardrobe = Mapper.Map(dalWardrobe);
        return bllWardrobe ?? null;
    }

    public async Task<ClothingItem> AddClothingItemToWardrobeAsync(Guid wardrobeId, ClothingItem clothingItem)
    {
        // verification
        var wardrobe = await ServiceRepository.FindAsync(wardrobeId, clothingItem.Wardrobe?.UserId ?? Guid.Empty);
        if (wardrobe == null) throw new ArgumentException("Wardrobe not found or does not belong to the user");

        clothingItem.WardrobeId = wardrobeId;
        
        // map to DAL and add to repository
        var dalClothingItem = _clothingItemMapper.Map(clothingItem);
        if (dalClothingItem == null) throw new InvalidOperationException("Failed to map clothing item in wardrobe service");
        
        _uow.ClothingItemRepository.Add(dalClothingItem);
        await _uow.SaveChangesAsync();
        
        // new ID to clothing item
        var addedDalItem = await _uow.ClothingItemRepository.FindAsync(dalClothingItem.Id);
        return _clothingItemMapper.Map(addedDalItem)!;
    }

    public async Task RemoveClothingItemFromWardrobeAsync(Guid wardrobeId, Guid clothingItemId)
    {
        var dalClothingItem = await _uow.ClothingItemRepository.FindAsync(clothingItemId);
        if (dalClothingItem == null || dalClothingItem.WardrobeId != wardrobeId)
        {
            throw new ArgumentException("Clothing item not found or doesn't belong to the specified wardrobe");
        }
        // delete
        await _uow.ClothingItemRepository.RemoveAsync(clothingItemId);
        await _uow.SaveChangesAsync();
        
        // TODO: user stats update
    }

    public async Task<IEnumerable<ClothingItem>> GetItemsByTypeAsync(Guid wardrobeId, ClothingType type)
    {
        var wardrobe = await ServiceRepository.FindAsync(wardrobeId);
        if (wardrobe == null) return Enumerable.Empty<ClothingItem>();
        
        var dalItems = await _uow.ClothingItemRepository.GetClothingItemsByTypeAsync(type, wardrobe.UserId);
        // filter
        dalItems = dalItems.Where(i => i.WardrobeId == wardrobeId);
        
        return dalItems.Select(i => _clothingItemMapper.Map(i)!).ToList();
    }

    public async Task<IEnumerable<ClothingItem>> GetItemsBySeasonAsync(Guid wardrobeId, ClothingSeason season)
    {
        var wardrobe = await ServiceRepository.FindAsync(wardrobeId);
        if (wardrobe == null) return Enumerable.Empty<ClothingItem>();
        
        var dalItems = await _uow.ClothingItemRepository.GetClothingItemsBySeasonAsync(season, wardrobe.UserId);
        // filter
        dalItems = dalItems.Where(i => i.WardrobeId == wardrobeId);
        
        return dalItems.Select(i => _clothingItemMapper.Map(i)!).ToList();
    }

    public async Task<IEnumerable<ClothingItem>> GetItemsByStyleAsync(Guid wardrobeId, ClothingStyle style)
    {
        var wardrobe = await ServiceRepository.FindAsync(wardrobeId);
        if (wardrobe == null) return Enumerable.Empty<ClothingItem>();
        
        var dalItems = await _uow.ClothingItemRepository.GetClothingItemsByStyleAsync(style, wardrobe.UserId);
        // filter
        dalItems = dalItems.Where(i => i.WardrobeId == wardrobeId);
        
        return dalItems.Select(i => _clothingItemMapper.Map(i)!).ToList();
    }

    public async Task<IEnumerable<ClothingItem>> GetItemsByColorAsync(Guid wardrobeId, ClothingColor color)
    {
        var wardrobe = await ServiceRepository.FindAsync(wardrobeId);
        if (wardrobe == null) return Enumerable.Empty<ClothingItem>();
        
        var dalItems = await _uow.ClothingItemRepository.GetClothingItemsByColorAsync(color, wardrobe.UserId);
        // filter
        dalItems = dalItems.Where(i => i.WardrobeId == wardrobeId);
        
        return dalItems.Select(i => _clothingItemMapper.Map(i)!).ToList();
    }
    
    public async Task<Dictionary<ClothingType, int>> GetWardrobeStatisticsAsync(Guid wardrobeId)
    {
        var wardrobe = await GetWardrobeWithItemsAsync(wardrobeId, Guid.Empty);
        if (wardrobe?.ClothingItems == null || wardrobe.ClothingItems.Count == 0)
        {
            return new Dictionary<ClothingType, int>();
        }
        
        // calculate statistics by type
        return wardrobe.ClothingItems
            .GroupBy(i => i.Type)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}