using APP.DAL.Contracts;
using APP.DAL.DTO;
using APP.DAL.EF.Mappers;
using BASE.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace APP.DAL.EF.Repositories;

public class FavouriteRepository : BaseRepository<Favourite, Domain.Favourite>, IFavouriteRepository
{
    public FavouriteRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new FavouriteUowMapper())
    {
    }
    
    public override async Task<IEnumerable<Favourite>> AllAsync(Guid userId = default)
    {
        var query = GetQuery(userId);
        query = query.Include(f => f.Outfit).Include(f => f.User);
        return (await query.ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public async Task<IEnumerable<Favourite>> GetFavouritesByUserAsync(Guid userId)
    {
        var query = GetQuery(userId);
        query = query.Include(f => f.Outfit);
        return (await query.ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public async Task<bool> IsFavouriteAsync(Guid outfitId, Guid userId)
    {
        return await RepositoryDbContext.Set<Domain.Favourite>()
            .AnyAsync(f => f.OutfitId == outfitId && f.UserId == userId);
    }

    public async Task<Favourite?> GetFavouriteByOutfitAndUserAsync(Guid outfitId, Guid userId)
    {
        return Mapper.Map(await RepositoryDbSet
            .Where(f => f.OutfitId == outfitId && f.UserId == userId)
            .FirstOrDefaultAsync());
    }

    public async Task<bool> ToggleFavouriteAsync(Guid outfitId, Guid userId)
    {
        var existing = await RepositoryDbSet
            .Where(f => f.OutfitId == outfitId && f.UserId == userId)
            .FirstOrDefaultAsync();

        if (existing != null)
        {
            RepositoryDbSet.Remove(existing);
            await RepositoryDbContext.SaveChangesAsync();
            return false; // Remove from favorites
        }
        else
        {
            var newFavourite = new Domain.Favourite
            {
                OutfitId = outfitId,
                UserId = userId
            };
            RepositoryDbSet.Add(newFavourite);
            await RepositoryDbContext.SaveChangesAsync();
            return true; // Add to favorites
        }
    }
}