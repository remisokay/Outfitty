using BASE.BLL.Contracts;

namespace APP.BLL.Contracts;

public interface IPlannerEntryService : IBaseService<DTO.PlannerEntry>
{
    // Calendar operations
    Task<IEnumerable<DTO.PlannerEntry>> GetUserEntriesForDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<DTO.PlannerEntry>> GetUserEntriesForMonthAsync(Guid userId, int year, int month);
    Task<DTO.PlannerEntry?> GetUserEntryForDateAsync(Guid userId, DateTime date);
    
    // Planning helpers
    Task<DTO.Outfit?> GetPlannedOutfitForDateAsync(Guid userId, DateTime date);
    Task<bool> IsOutfitPlannedForDateAsync(Guid outfitId, DateTime date);
}