using BASE.BLL.Contracts;

namespace APP.BLL.Contracts;

public interface IPlannerEntryService : IBaseService<DTO.PlannerEntry>
{
    // Calendar operations
    Task<IEnumerable<DTO.PlannerEntry>> GetUserEntriesForDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<DTO.PlannerEntry>> GetUserEntriesForMonthAsync(Guid userId, int year, int month);
    Task<DTO.PlannerEntry?> GetUserEntryForDateAsync(Guid userId, DateTime date);
    Task<IEnumerable<DTO.PlannerEntry>> GetUserPlannerEntriesAsync(Guid userId);
    Task<IEnumerable<DTO.PlannerEntry>> GetTodayEntriesAsync(Guid userId);
    Task<IEnumerable<DTO.PlannerEntry>> GetUpcomingEntriesAsync(Guid userId, int maxResults = 10);

    Task<DTO.PlannerEntry> CreatePlannerEntryAsync(DTO.PlannerEntry plannerEntry);
    Task<DTO.PlannerEntry?> UpdatePlannerEntryAsync(DTO.PlannerEntry plannerEntry);
    Task<bool> DeletePlannerEntryAsync(Guid entryId, Guid userId);
    
    // Planning helpers
    Task<DTO.Outfit?> GetPlannedOutfitForDateAsync(Guid userId, DateTime date);
    Task<bool> IsOutfitPlannedForDateAsync(Guid outfitId, DateTime date);
    Task<bool> HasPlannedOutfitForDateAsync(Guid userId, DateTime date);
}