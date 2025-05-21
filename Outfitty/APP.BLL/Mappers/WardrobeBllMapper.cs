using APP.BLL.DTO;
using BASE.Contracts;
using Domain.identity;

namespace APP.BLL.Mappers;

public class WardrobeBllMapper : IMapper<Wardrobe, APP.DAL.DTO.Wardrobe>
{
    public Wardrobe? Map(DAL.DTO.Wardrobe? entity)
    {
        if (entity == null) return null;

        var result = new Wardrobe
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            UserId = entity.UserId,
            
            User = entity.User == null ? null : new AppUser
            {
                Id = entity.User.Id,
                Username = entity.User.Username
            },
            
            ClothingItems = entity.ClothingItems?.Select(ci => new ClothingItem
            {
                Id = ci.Id,
                Name = ci.Name,
                Type = ci.Type,
                Style = ci.Style,
                Season = ci.Season,
                PrimaryColor = ci.PrimaryColor,
                SecondaryColor = ci.SecondaryColor,
                WardrobeId = ci.WardrobeId,
                ImageMetadataId = ci.ImageMetadataId
            }).ToList()
        };

        return result;
    }

    public DAL.DTO.Wardrobe? Map(Wardrobe? entity)
    {
        if (entity == null) return null;

        var result = new APP.DAL.DTO.Wardrobe
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            UserId = entity.UserId
        };

        return result;
    }
}