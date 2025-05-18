using APP.DAL.Contracts;
using APP.DAL.DTO;
using APP.DAL.EF.Mappers;
using BASE.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace APP.DAL.EF.Repositories;

public class PlannerEntryRepository : BaseRepository<PlannerEntry, Domain.PlannerEntry>, IPlannerEntryRepository
{
    public PlannerEntryRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new PlannerEntryUowMapper())
    {
    }
    
    public override async Task<IEnumerable<PlannerEntry>> AllAsync(Guid userId = default)
    {
        var query = GetQuery(userId);
        query = query.Include(p => p.Outfit).Include(p => p.User);
        return (await query.ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public override async Task<PlannerEntry?> FindAsync(Guid id, Guid userId = default)
    {
        return Mapper.Map(await RepositoryDbSet
            .Include(p => p.Outfit)
            .Include(p => p.User)
            .Where(p => p.Id == id && p.UserId == userId)
            .FirstOrDefaultAsync());
    }

    public async Task<IEnumerable<PlannerEntry>> GetPlannerEntriesByUserAsync(Guid userId)
    {
        var query = GetQuery(userId);
        query = query.Include(p => p.Outfit).OrderBy(p => p.Date);
        return (await query.ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public async Task<IEnumerable<PlannerEntry>> GetPlannerEntriesByDateRangeAsync(DateTime startDate, DateTime endDate, Guid userId)
    {
        var query = GetQuery(userId);
        return (await query
            .Include(p => p.Outfit)
            .Where(p => p.Date >= startDate && p.Date <= endDate)
            .OrderBy(p => p.Date)
            .ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public async Task<IEnumerable<PlannerEntry>> GetPlannerEntriesForDateAsync(DateTime date, Guid userId)
    {
        var query = GetQuery(userId);
        return (await query
            .Include(p => p.Outfit)
            .Where(p => p.Date.Date == date.Date)
            .OrderBy(p => p.Time)
            .ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public async Task<PlannerEntry?> GetPlannerEntryWithOutfitAsync(Guid entryId, Guid userId)
    {
        return Mapper.Map(await RepositoryDbSet
            .Include(p => p.Outfit)
            .ThenInclude(o => o!.OutfitItems!)
            .ThenInclude(oi => oi.ClothingItem)
            .Where(p => p.Id == entryId && p.UserId == userId)
            .FirstOrDefaultAsync());
    }

    public async Task<bool> HasOutfitForDateAsync(DateTime date, Guid userId)
    {
        return await RepositoryDbContext.Set<Domain.PlannerEntry>()
            .AnyAsync(p => p.Date.Date == date.Date && p.UserId == userId);
    }
}