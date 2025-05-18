using APP.DAL.DTO;
using BASE.Contracts;
using Domain.identity;

namespace APP.DAL.EF.Mappers;

public class FavouriteUowMapper : IMapper<Favourite, Domain.Favourite>
{
    public Favourite? Map(Domain.Favourite? entity)
    {
        if (entity == null) return null;

        var result = new Favourite()
        {
            Id = entity.Id,
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

    public Domain.Favourite? Map(Favourite? entity)
    {
        if (entity == null) return null;

        var result = new Domain.Favourite()
        {
            Id = entity.Id,
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