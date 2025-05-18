using APP.DAL.DTO;
using BASE.Contracts;
using Domain.identity;

namespace APP.DAL.EF.Mappers;

public class PlannerEntryUowMapper : IMapper<PlannerEntry, Domain.PlannerEntry>
{
    public PlannerEntry? Map(Domain.PlannerEntry? entity)
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
            User = entity.User == null ? null : new AppUser()
            {
                Id = entity.User.Id,
                Username = entity.User.Username
            },
            Outfit = entity.Outfit == null ? null : new Outfit()
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

    public Domain.PlannerEntry? Map(PlannerEntry? entity)
    {
        if (entity == null) return null;

        var result = new Domain.PlannerEntry()
        {
            Id = entity.Id,
            Date = entity.Date,
            Title = entity.Title,
            Comment = entity.Comment,
            Time = entity.Time,
            UserId = entity.UserId,
            OutfitId = entity.OutfitId,
            User = entity.User == null ? null : new AppUser()
            {
                Id = entity.User.Id,
                Username = entity.User.Username
            },
            Outfit = entity.Outfit == null ? null : new Domain.Outfit()
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
}