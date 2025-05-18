using APP.DAL.Contracts;
using APP.DAL.DTO;
using APP.DAL.EF.Mappers;
using BASE.DAL.EF;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace APP.DAL.EF.Repositories;

public class OutfitRepository : BaseRepository<Outfit, Domain.Outfit>, IOutfitRepository
{
    public OutfitRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new OutfitUowMapper())
    {
    }
    
    public override async Task<IEnumerable<Outfit>> AllAsync(Guid userId = default)
    {
        var query = GetQuery(userId);
        query = query.Include(o => o.User);
        return (await query.ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public override async Task<Outfit?> FindAsync(Guid id, Guid userId = default)
    {
        return Mapper.Map(await RepositoryDbSet
            .Include(o => o.User)
            .Where(o => o.Id == id && o.UserId == userId)
            .FirstOrDefaultAsync());
    }

    public async Task<IEnumerable<Outfit>> GetOutfitsByUserAsync(Guid userId)
    {
        var query = GetQuery(userId);
        return (await query.ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public async Task<IEnumerable<Outfit>> GetOutfitsBySeasonAsync(ClothingSeason season, Guid userId)
    {
        var query = GetQuery(userId);
        return (await query.Where(o => o.Season == season).ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public async Task<IEnumerable<Outfit>> GetOutfitsByStyleAsync(ClothingStyle style, Guid userId)
    {
        var query = GetQuery(userId);
        return (await query.Where(o => o.Style == style).ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public async Task<Outfit?> GetOutfitWithItemsAsync(Guid outfitId, Guid userId)
    {
        return Mapper.Map(await RepositoryDbSet
            .Include(o => o.OutfitItems!)
            .ThenInclude(oi => oi.ClothingItem)
            .ThenInclude(ci => ci!.ImageMetadata)
            .Where(o => o.Id == outfitId && o.UserId == userId)
            .FirstOrDefaultAsync());
    }

    public async Task<Outfit?> GetOutfitWithAllDetailsAsync(Guid outfitId, Guid userId)
    {
        return Mapper.Map(await RepositoryDbSet
            .Include(o => o.OutfitItems!)
            .ThenInclude(oi => oi.ClothingItem)
            .ThenInclude(ci => ci!.ImageMetadata)
            .Include(o => o.Favourites)
            .Include(o => o.PlannerEntries)
            .Where(o => o.Id == outfitId && o.UserId == userId)
            .FirstOrDefaultAsync());
    }

    public async Task<IEnumerable<Outfit>> GetFavoriteOutfitsAsync(Guid userId)
    {
        return (await RepositoryDbContext.Set<Domain.Outfit>()
            .Include(o => o.Favourites)
            .Where(o => o.UserId == userId && o.Favourites!.Any(f => f.UserId == userId))
            .ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public async Task<int> GetOutfitsCountByUserAsync(Guid userId)
    {
        var query = GetQuery(userId);
        return await query.CountAsync();
    }
}