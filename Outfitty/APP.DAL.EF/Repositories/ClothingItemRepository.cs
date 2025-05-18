using APP.DAL.Contracts;
using APP.DAL.DTO;
using APP.DAL.EF.Mappers;
using BASE.DAL.EF;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace APP.DAL.EF.Repositories;

public class ClothingItemRepository : BaseRepository<ClothingItem, Domain.ClothingItem>, IClothingItemRepository
{
    public ClothingItemRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new ClothingItemUowMapper())
    {
    }
    
    public override async Task<IEnumerable<ClothingItem>> AllAsync(Guid userId = default)
    {
        var query = GetQuery(userId);
        query = query.Include(c => c.Wardrobe)
            .Include(c => c.ImageMetadata);
        return (await query.ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public override async Task<ClothingItem?> FindAsync(Guid id, Guid userId = default)
    {
        return Mapper.Map(await RepositoryDbSet
            .Include(c => c.Wardrobe)
            .Include(c => c.ImageMetadata)
            .Where(c => c.Id == id && c.Wardrobe!.UserId == userId)
            .FirstOrDefaultAsync());
    }

    public async Task<IEnumerable<ClothingItem>> GetClothingItemsByWardrobeAsync(Guid wardrobeId, Guid userId)
    {
        return (await RepositoryDbSet
            .Include(c => c.ImageMetadata)
            .Where(c => c.WardrobeId == wardrobeId && c.Wardrobe!.UserId == userId)
            .ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public async Task<IEnumerable<ClothingItem>> GetClothingItemsByTypeAsync(ClothingType type, Guid userId)
    {
        return (await RepositoryDbContext.Set<Domain.ClothingItem>()
            .Include(c => c.Wardrobe)
            .Include(c => c.ImageMetadata)
            .Where(c => c.Type == type && c.Wardrobe!.UserId == userId)
            .ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public async Task<IEnumerable<ClothingItem>> GetClothingItemsBySeasonAsync(ClothingSeason season, Guid userId)
    {
        return (await RepositoryDbContext.Set<Domain.ClothingItem>()
            .Include(c => c.Wardrobe)
            .Include(c => c.ImageMetadata)
            .Where(c => c.Season == season && c.Wardrobe!.UserId == userId)
            .ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public async Task<IEnumerable<ClothingItem>> GetClothingItemsByStyleAsync(ClothingStyle style, Guid userId)
    {
        return (await RepositoryDbContext.Set<Domain.ClothingItem>()
            .Include(c => c.Wardrobe)
            .Include(c => c.ImageMetadata)
            .Where(c => c.Style == style && c.Wardrobe!.UserId == userId)
            .ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public async Task<IEnumerable<ClothingItem>> GetClothingItemsByColorAsync(ClothingColor color, Guid userId)
    {
        return (await RepositoryDbContext.Set<Domain.ClothingItem>()
            .Include(c => c.Wardrobe)
            .Include(c => c.ImageMetadata)
            .Where(c => (c.PrimaryColor == color || c.SecondaryColor == color) && c.Wardrobe!.UserId == userId)
            .ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public async Task<ClothingItem?> GetClothingItemWithImageAsync(Guid clothingItemId, Guid userId)
    {
        return Mapper.Map(await RepositoryDbSet
            .Include(c => c.ImageMetadata)
            .Include(c => c.Wardrobe)
            .Where(c => c.Id == clothingItemId && c.Wardrobe!.UserId == userId)
            .FirstOrDefaultAsync());
    }

    public async Task<int> GetClothingItemsCountByUserAsync(Guid userId)
    {
        return await RepositoryDbContext.Set<Domain.ClothingItem>()
            .Include(c => c.Wardrobe)
            .Where(c => c.Wardrobe!.UserId == userId)
            .CountAsync();
    }
}