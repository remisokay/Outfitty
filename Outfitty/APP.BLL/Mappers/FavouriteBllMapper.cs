using APP.BLL.DTO;
using BASE.Contracts;
using Domain.identity;

namespace APP.BLL.Mappers;

public class FavouriteBllMapper : IMapper<Favourite, APP.DAL.DTO.Favourite>
{
    public Favourite? Map(DAL.DTO.Favourite? entity)
    {
        if (entity == null) return null;

        var result = new Favourite
        {
            Id = entity.Id,
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
            },
        };

        return result;
    }

    public DAL.DTO.Favourite? Map(Favourite? entity)
    {
        if (entity == null) return null;

        var result = new APP.DAL.DTO.Favourite
        {
            Id = entity.Id,
            UserId = entity.UserId,
            OutfitId = entity.OutfitId
        };

        return result;

    }
}