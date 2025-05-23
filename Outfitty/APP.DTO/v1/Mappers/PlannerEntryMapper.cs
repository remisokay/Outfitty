using BASE.Contracts;

namespace APP.DTO.v1.Mappers;

public class PlannerEntryMapper : IMapper<PlannerEntry, BLL.DTO.PlannerEntry>
{
    public PlannerEntry? Map(BLL.DTO.PlannerEntry? entity)
    {
        if (entity == null) return null;

        var result = new PlannerEntry()
        {
            Id = entity.Id,
            Date = entity.Date,
            Title = entity.Title,
            Comment = entity.Comment,
            Time = entity.Time,
            UserId = entity.UserId,
            OutfitId = entity.OutfitId,
            
            OutfitName = entity.Outfit?.Name,
            IsUpcoming = entity.Upcoming,
            IsToday = entity.Today
        };
        return result;
    }

    public BLL.DTO.PlannerEntry? Map(PlannerEntry? entity)
    {
        if (entity == null) return null;
        
        var result = new APP.BLL.DTO.PlannerEntry()
        {
            Id = entity.Id,
            Date = entity.Date,
            Title = entity.Title,
            Comment = entity.Comment,
            Time = entity.Time,
            UserId = entity.UserId,
            OutfitId = entity.OutfitId
        };
        
        return result;
    }
    
    public APP.BLL.DTO.PlannerEntry Map(PlannerEntryCreate entity)
    {
        var result = new APP.BLL.DTO.PlannerEntry()
        {
            Id = Guid.NewGuid(),
            Date = entity.Date,
            Title = entity.Title,
            Comment = entity.Comment,
            Time = entity.Time,
            UserId = entity.UserId,
            OutfitId = entity.OutfitId
        };
        
        return result;
    }
}