using APP.BLL.Contracts;
using APP.BLL.DTO;
using APP.DAL.Contracts;
using BASE.BLL;
using BASE.Contracts;

namespace APP.BLL.Services;

public class PlannerEntryService : BaseService<PlannerEntry, APP.DAL.DTO.PlannerEntry, IPlannerEntryRepository>, IPlannerEntryService
{
    private readonly IAppUow _uow;
    private readonly IMapper<Outfit, DAL.DTO.Outfit> _outfitMapper;

    public PlannerEntryService(
        IAppUow serviceUow,
        IMapper<PlannerEntry, DAL.DTO.PlannerEntry> mapper,
        IMapper<Outfit, DAL.DTO.Outfit> outfitMapper)
        : base(serviceUow, serviceUow.PlannerEntryRepository, mapper)
    {
        _uow = serviceUow;
        _outfitMapper = outfitMapper;
    }

    public async Task<IEnumerable<PlannerEntry>> GetUserEntriesForDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date must go before the end date");
        
        var dalEntries = await ServiceRepository.GetPlannerEntriesByDateRangeAsync(startDate, endDate, userId);
        var bllEntries = dalEntries.Select(entry => Mapper.Map(entry)!).ToList();
        // business logic for later
        return bllEntries.OrderBy(e => e.Date).ThenBy(e => e.Time);
    }

    public async Task<IEnumerable<PlannerEntry>> GetUserEntriesForMonthAsync(Guid userId, int year, int month)
    {
        if (month is < 1 or > 12)
            throw new ArgumentException("Month must be between 1 and 12");
        

        if (year is < 1900 or > 2100)
            throw new ArgumentException("Year must be between 1900 and 2100");
        
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        return await GetUserEntriesForDateRangeAsync(userId, startDate, endDate);
    }

    public async Task<PlannerEntry?> GetUserEntryForDateAsync(Guid userId, DateTime date)
    {
        var dalEntries = await ServiceRepository.GetPlannerEntriesForDateAsync(date.Date, userId);
        
        if (!dalEntries.Any()) return null;
        
        var earliestEntry = dalEntries
            .OrderBy(e => e.Time ?? TimeSpan.Zero)
            .First();

        var bllEntry = Mapper.Map(earliestEntry)!;
        // more business logic if needed
        
        return bllEntry;
    }

    public async Task<Outfit?> GetPlannedOutfitForDateAsync(Guid userId, DateTime date)
    {
        var plannerEntry = await GetUserEntryForDateAsync(userId, date);
        if (plannerEntry?.OutfitId == null) return null;
        
        var dalOutfit = await _uow.OutfitRepository.FindAsync(plannerEntry.OutfitId, userId);
        if (dalOutfit == null) return null;
        
        var bllOutfit = _outfitMapper.Map(dalOutfit);
        
        if (bllOutfit != null)
        {
            bllOutfit.IsFavourite = await _uow.FavouriteRepository.IsFavouriteAsync(bllOutfit.Id, userId);
        }

        return bllOutfit;
    }

    public async Task<bool> IsOutfitPlannedForDateAsync(Guid outfitId, DateTime date)
    {
        var dalOutfit = await _uow.OutfitRepository.FindAsync(outfitId);
        if (dalOutfit == null) return false;
        
        var entriesForDate = await ServiceRepository.GetPlannerEntriesForDateAsync(date.Date, dalOutfit.UserId);
        return entriesForDate.Any(entry => entry.OutfitId == outfitId);
    }
    
    public async Task<IEnumerable<PlannerEntry>> GetUserPlannerEntriesAsync(Guid userId)
    {
        var dalEntries = await ServiceRepository.GetPlannerEntriesByUserAsync(userId);
        var bllEntries = dalEntries.Select(entry => Mapper.Map(entry)!).ToList();

        return bllEntries.OrderBy(e => e.Date).ThenBy(e => e.Time);
    }
    
    public async Task<PlannerEntry> CreatePlannerEntryAsync(PlannerEntry plannerEntry)
    {
        await ValidatePlannerEntryAsync(plannerEntry);

        var dalEntry = Mapper.Map(plannerEntry);
        if (dalEntry == null)
            throw new InvalidOperationException("Failed to map planner entry");
        
        dalEntry.Id = Guid.NewGuid();
        
        ServiceRepository.Add(dalEntry);
        await _uow.SaveChangesAsync();
        
        var createdEntry = await ServiceRepository.FindAsync(dalEntry.Id, plannerEntry.UserId);
        var bllCreatedEntry = Mapper.Map(createdEntry)!;

        return bllCreatedEntry;
    }
    
    public async Task<PlannerEntry?> UpdatePlannerEntryAsync(PlannerEntry plannerEntry)
    {
        var existingEntry = await ServiceRepository.FindAsync(plannerEntry.Id, plannerEntry.UserId);
        if (existingEntry == null) return null;
        await ValidatePlannerEntryAsync(plannerEntry);

        var dalEntry = Mapper.Map(plannerEntry);
        if (dalEntry == null)
            throw new InvalidOperationException("Failed to map planner entry");
        
        await ServiceRepository.UpdateAsync(dalEntry);
        await _uow.SaveChangesAsync();

        var updatedEntry = await ServiceRepository.FindAsync(plannerEntry.Id, plannerEntry.UserId);
        var bllUpdatedEntry = Mapper.Map(updatedEntry)!;

        return bllUpdatedEntry;
    }
    
    public async Task<bool> DeletePlannerEntryAsync(Guid entryId, Guid userId)
    {
        var existingEntry = await ServiceRepository.FindAsync(entryId, userId);
        if (existingEntry == null) return false;
        
        await ServiceRepository.RemoveAsync(entryId, userId);
        await _uow.SaveChangesAsync();

        return true;
    }
    
    public async Task<IEnumerable<PlannerEntry>> GetUpcomingEntriesAsync(Guid userId, int maxResults = 10)
    {
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddMonths(3); // Look ahead 3 months

        var entries = await GetUserEntriesForDateRangeAsync(userId, startDate, endDate);
        return entries
            .Where(e => e.Upcoming)
            .Take(maxResults)
            .ToList();
    }
    
    public async Task<IEnumerable<PlannerEntry>> GetTodayEntriesAsync(Guid userId)
    {
        var entries = await ServiceRepository.GetPlannerEntriesForDateAsync(DateTime.Today, userId);
        var bllEntries = entries.Select(entry => Mapper.Map(entry)!).ToList();

        return bllEntries.OrderBy(e => e.Time ?? TimeSpan.Zero);
    }
    
    public async Task<bool> HasPlannedOutfitForDateAsync(Guid userId, DateTime date)
    {
        return await ServiceRepository.HasOutfitForDateAsync(date.Date, userId);
    }
    
    
    
    
    // helper methods
    private async Task ValidatePlannerEntryAsync(PlannerEntry plannerEntry)
    {
        if (string.IsNullOrWhiteSpace(plannerEntry.Title))
            throw new ArgumentException("Title is required");
        
        if (plannerEntry.UserId == Guid.Empty)
            throw new ArgumentException("User ID is required");

        if (plannerEntry.OutfitId == Guid.Empty)
            throw new ArgumentException("Outfit ID is required");
        
        var outfit = await _uow.OutfitRepository.FindAsync(plannerEntry.OutfitId, plannerEntry.UserId);
        if (outfit == null)
            throw new ArgumentException("Outfit not found or does not belong to the user");
        
        if (plannerEntry.Time.HasValue && (plannerEntry.Time.Value < TimeSpan.Zero || plannerEntry.Time.Value >= TimeSpan.FromDays(1)))
            throw new ArgumentException("Time must be between 00:00 and 23:59");
    }
    
    
}