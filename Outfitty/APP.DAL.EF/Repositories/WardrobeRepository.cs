using APP.DAL.Contracts;
using APP.DAL.DTO;
using APP.DAL.EF.Mappers;
using BASE.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace APP.DAL.EF.Repositories;

public class WardrobeRepository : BaseRepository<Wardrobe, Domain.Wardrobe>, IWardrobeRepository
{
    public WardrobeRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new WardrobeUowMapper())
    {
    }
    
    // Returns all wardrobes that belong to the specified user
    public override async Task<IEnumerable<Wardrobe>> AllAsync(Guid userId = default)
    {
        var query = GetQuery(userId);
        query = query.Include(w => w.User);
        return (await query.ToListAsync()).Select(e => Mapper.Map(e)!); // transfer
    }
    
    // Returns the specific wardrobe with the given ID
    public override async Task<Wardrobe?> FindAsync(Guid id, Guid userId = default)
    {
        return Mapper.Map(await RepositoryDbSet
            .Include(w => w.User)
            .Where(w => w.Id == id && w.UserId == userId)
            .FirstOrDefaultAsync());
    }

    public async Task<IEnumerable<Wardrobe>> GetWardrobesByUserAsync(Guid userId)
    {
        var query = GetQuery(userId);
        return (await query.ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public async Task<Wardrobe?> GetWardrobeWithClothingItemsAsync(Guid wardrobeId, Guid userId)
    {
        return Mapper.Map(await RepositoryDbSet
            .Include(w => w.ClothingItems)
            .Where(w => w.Id == wardrobeId && w.UserId == userId)
            .FirstOrDefaultAsync());
    }

    public async Task<int> GetClothingItemsCountAsync(Guid wardrobeId, Guid userId)
    {
        return await RepositoryDbSet
            .Where(w => w.Id == wardrobeId && w.UserId == userId)
            .SelectMany(w => w.ClothingItems!)
            .CountAsync();
    }
}

// Async allows the thread to handle other requests while waiting for database results