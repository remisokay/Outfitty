using BASE.DAL.Contracts;

namespace APP.DAL.Contracts;

public interface IPlannerEntryRepository : IBaseRepository<DTO.PlannerEntry>, IPlannerEntryRepositoryCustom
{
    
}

public interface IPlannerEntryRepositoryCustom
{
    Task<IEnumerable<DTO.PlannerEntry>> GetPlannerEntriesByUserAsync(Guid userId);
    Task<IEnumerable<DTO.PlannerEntry>> GetPlannerEntriesByDateRangeAsync(DateTime startDate, DateTime endDate, Guid userId);
    Task<IEnumerable<DTO.PlannerEntry>> GetPlannerEntriesForDateAsync(DateTime date, Guid userId);
    Task<DTO.PlannerEntry?> GetPlannerEntryWithOutfitAsync(Guid entryId, Guid userId);
    Task<bool> HasOutfitForDateAsync(DateTime date, Guid userId);
}