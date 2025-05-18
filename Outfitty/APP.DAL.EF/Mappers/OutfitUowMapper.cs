using APP.DAL.DTO;
using BASE.Contracts;
using Domain.identity;

namespace APP.DAL.EF.Mappers;

public class OutfitUowMapper : IMapper<Outfit, Domain.Outfit>
{
    public Outfit? Map(Domain.Outfit? entity)
    {
        if (entity == null) return null;
        
        var result = new Outfit()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Season = entity.Season,
            Style = entity.Style,
            UserId = entity.UserId,
            User = entity.User == null ? null : new AppUser()
            {
                Id = entity.User.Id,
                Username = entity.User.Username,
            },
            OutfitItems = entity.OutfitItems?.Select(oi => new OutfitItem()
            {
                Id = oi.Id,
                OutfitId = oi.OutfitId,
                DisplayOrder = oi.DisplayOrder,
                ClothingItemId = oi.ClothingItemId,
                Outfit = null, // why null?
                ClothingItem = oi.ClothingItem == null ? null : new ClothingItem()
                {
                    Id = oi.ClothingItem.Id,
                    Name = oi.ClothingItem.Name,
                    Type = oi.ClothingItem.Type,
                    Style = oi.ClothingItem.Style,
                    Season = oi.ClothingItem.Season,
                    PrimaryColor = oi.ClothingItem.PrimaryColor,
                    SecondaryColor = oi.ClothingItem.SecondaryColor,
                    WardrobeId = oi.ClothingItem.WardrobeId,
                    ImageMetadataId = oi.ClothingItem.ImageMetadataId
                }
            }).ToList(),
            PlannerEntries = entity.PlannerEntries?.Select(pe => new PlannerEntry()
            {
                Id = pe.Id,
                Date = pe.Date,
                Title = pe.Title,
                Comment = pe.Comment,
                Time = pe.Time,
                UserId = pe.UserId,
                OutfitId = pe.OutfitId,
                Outfit = null
            }).ToList(),
            Favourites = entity.Favourites?.Select(f => new Favourite()
            {
                Id = f.Id,
                UserId = f.UserId,
                OutfitId = f.OutfitId,
                Outfit = null
            }).ToList()
        };
        return result;
    }

    public Domain.Outfit? Map(Outfit? entity)
    {
        if (entity == null) return null;

        var result = new Domain.Outfit()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Season = entity.Season,
            Style = entity.Style,
            UserId = entity.UserId,
            User = entity.User == null ? null : new AppUser()
            {
                Id = entity.User.Id,
                Username = entity.User.Username
            },
            OutfitItems = entity.OutfitItems?.Select(oi => new Domain.OutfitItem()
            {
                Id = oi.Id,
                OutfitId = oi.OutfitId,
                DisplayOrder = oi.DisplayOrder,
                ClothingItemId = oi.ClothingItemId,
                Outfit = null,
                ClothingItem = oi.ClothingItem == null ? null : new Domain.ClothingItem()
                {
                    Id = oi.ClothingItem.Id,
                    Name = oi.ClothingItem.Name,
                    Type = oi.ClothingItem.Type,
                    Style = oi.ClothingItem.Style,
                    Season = oi.ClothingItem.Season,
                    PrimaryColor = oi.ClothingItem.PrimaryColor,
                    SecondaryColor = oi.ClothingItem.SecondaryColor,
                    WardrobeId = oi.ClothingItem.WardrobeId,
                    ImageMetadataId = oi.ClothingItem.ImageMetadataId
                }
            }).ToList(),
            PlannerEntries = entity.PlannerEntries?.Select(pe => new Domain.PlannerEntry()
            {
                Id = pe.Id,
                Date = pe.Date,
                Title = pe.Title,
                Comment = pe.Comment,
                Time = pe.Time,
                UserId = pe.UserId,
                OutfitId = pe.OutfitId,
                Outfit = null
            }).ToList(),
            Favourites = entity.Favourites?.Select(f => new Domain.Favourite()
            {
                Id = f.Id,
                UserId = f.UserId,
                OutfitId = f.OutfitId,
                Outfit = null
            }).ToList()
        };
        return result;
    }
}