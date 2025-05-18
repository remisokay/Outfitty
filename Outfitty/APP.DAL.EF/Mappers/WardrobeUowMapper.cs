using APP.DAL.DTO;
using BASE.Contracts;
using Domain.identity;

namespace APP.DAL.EF.Mappers;

public class WardrobeUowMapper : IMapper<Wardrobe, Domain.Wardrobe>
{
    public Wardrobe? Map(Domain.Wardrobe? entity)
    {
        if (entity == null) return null;

        var result = new Wardrobe()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            UserId = entity.UserId,
            User = entity.User == null ? null : new AppUser()
            {
                Id = entity.User.Id,
                Username = entity.User.Username
            },
            ClothingItems = entity.ClothingItems?.Select(c => new ClothingItem()
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                Style = c.Style,
                Season = c.Season,
                PrimaryColor = c.PrimaryColor,
                SecondaryColor = c.SecondaryColor,
                WardrobeId = c.WardrobeId,
                Wardrobe = null, // why null?
                ImageMetadataId = c.ImageMetadataId,
                ImageMetadata = null // ??
            }).ToList()
        };
        return result;
    }

    public Domain.Wardrobe? Map(Wardrobe? entity)
    {
        if (entity == null) return null;

        var result = new Domain.Wardrobe()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            UserId = entity.UserId,
            User = entity.User == null ? null : new AppUser()
            {
                Id = entity.User.Id,
                Username = entity.User.Username
            },
            ClothingItems = entity.ClothingItems?.Select(c => new Domain.ClothingItem()
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                Style = c.Style,
                Season = c.Season,
                PrimaryColor = c.PrimaryColor,
                SecondaryColor = c.SecondaryColor,
                WardrobeId = c.WardrobeId,
                Wardrobe = null, // why null?
                ImageMetadataId = c.ImageMetadataId,
                ImageMetadata = null // ??
            }).ToList()
        };
        return result;
    }
}