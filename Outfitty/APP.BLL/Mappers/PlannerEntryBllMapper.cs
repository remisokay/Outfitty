using APP.BLL.DTO;
using BASE.Contracts;
using Domain.identity;

namespace APP.BLL.Mappers;

public class PlannerEntryBllMapper : IMapper<PlannerEntry, APP.DAL.DTO.PlannerEntry>
{
    public PlannerEntry? Map(DAL.DTO.PlannerEntry? entity)
    {
        if (entity == null) return null;

        var result = new PlannerEntry
        {
            Id = entity.Id,
            Date = entity.Date,
            Title = entity.Title,
            Comment = entity.Comment,
            Time = entity.Time,
            UserId = entity.UserId,
            OutfitId = entity.OutfitId,
            
            User = entity.User == null ? null : new AppUser
            {
                Id = entity.User.Id,
                Username = entity.User.Username
            },
            
            Outfit = entity.Outfit == null ? null : new Outfit
            {
                Id = entity.Outfit.Id,
                Name = entity.Outfit.Name,
                Description = entity.Outfit.Description,
                Season = entity.Outfit.Season,
                Style = entity.Outfit.Style,
                UserId = entity.Outfit.UserId
            }
        };

        return result;
    }

    public DAL.DTO.PlannerEntry? Map(PlannerEntry? entity)
    {
        if (entity == null) return null;

        var result = new APP.DAL.DTO.PlannerEntry
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
}