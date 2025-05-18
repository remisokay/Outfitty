using APP.DAL.Contracts;
using APP.DAL.DTO;
using APP.DAL.EF.Mappers;
using BASE.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace APP.DAL.EF.Repositories;

public class OutfitItemRepository : BaseRepository<OutfitItem, Domain.OutfitItem>, IOutfitItemRepository
{
    public OutfitItemRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new OutfitItemUowMapper())
    {
    }
    
    public override async Task<IEnumerable<OutfitItem>> AllAsync(Guid userId = default)
    {
        var query = RepositoryDbSet.AsQueryable();
        // Filter by user through outfit relationship
        query = query.Include(oi => oi.Outfit).Include(oi => oi.ClothingItem)
            .Where(oi => oi.Outfit!.UserId == userId);
        return (await query.ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public override async Task<OutfitItem?> FindAsync(Guid id, Guid userId = default)
    {
        return Mapper.Map(await RepositoryDbSet
            .Include(oi => oi.Outfit)
            .Include(oi => oi.ClothingItem)
            .Where(oi => oi.Id == id && oi.Outfit!.UserId == userId)
            .FirstOrDefaultAsync());
    }

    public async Task<IEnumerable<OutfitItem>> GetOutfitItemsByOutfitAsync(Guid outfitId, Guid userId)
    {
        return (await RepositoryDbSet
            .Include(oi => oi.ClothingItem)
            .ThenInclude(ci => ci!.ImageMetadata)
            .Include(oi => oi.Outfit)
            .Where(oi => oi.OutfitId == outfitId && oi.Outfit!.UserId == userId)
            .OrderBy(oi => oi.DisplayOrder)
            .ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public async Task<IEnumerable<OutfitItem>> GetOutfitItemsByClothingItemAsync(Guid clothingItemId, Guid userId)
    {
        return (await RepositoryDbSet
            .Include(oi => oi.Outfit)
            .Include(oi => oi.ClothingItem)
            .Where(oi => oi.ClothingItemId == clothingItemId && oi.Outfit!.UserId == userId)
            .ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public async Task<OutfitItem?> GetOutfitItemWithDetailsAsync(Guid outfitItemId, Guid userId)
    {
        return Mapper.Map(await RepositoryDbSet
            .Include(oi => oi.Outfit)
            .Include(oi => oi.ClothingItem)
            .ThenInclude(ci => ci!.ImageMetadata)
            .Where(oi => oi.Id == outfitItemId && oi.Outfit!.UserId == userId)
            .FirstOrDefaultAsync());
    }

    public async Task<bool> ReorderOutfitItemsAsync(Guid outfitId, Dictionary<Guid, int> itemOrderMap, Guid userId)
    {
        var outfitItems = await RepositoryDbSet
            .Include(oi => oi.Outfit)
            .Where(oi => oi.OutfitId == outfitId && oi.Outfit!.UserId == userId)
            .ToListAsync();

        if (outfitItems.Count == 0) return false;

        foreach (var item in outfitItems)
        {
            if (itemOrderMap.ContainsKey(item.Id))
            {
                item.DisplayOrder = itemOrderMap[item.Id];
            }
        }

        await RepositoryDbContext.SaveChangesAsync();
        return true;
    }
}