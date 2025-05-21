using APP.BLL.DTO;
using BASE.Contracts;
using Domain.identity;

namespace APP.BLL.Mappers;

public class OutfitBllMapper : IMapper<Outfit, APP.DAL.DTO.Outfit>
{
    public Outfit? Map(DAL.DTO.Outfit? entity)
    {
        if (entity == null) return null;

        var result = new Outfit
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Season = entity.Season,
            Style = entity.Style,
            UserId = entity.UserId,
            
            User = entity.User == null ? null : new AppUser
            {
                Id = entity.User.Id,
                Username = entity.User.Username
            },
            
            OutfitItems = entity.OutfitItems?.Select(oi => new OutfitItem
            {
                Id = oi.Id,
                OutfitId = oi.OutfitId,
                DisplayOrder = oi.DisplayOrder,
                ClothingItemId = oi.ClothingItemId,
                ClothingItem = oi.ClothingItem == null ? null : new ClothingItem
                {
                    Id = oi.ClothingItem.Id,
                    Name = oi.ClothingItem.Name,
                    Type = oi.ClothingItem.Type,
                    Style = oi.ClothingItem.Style,
                    Season = oi.ClothingItem.Season,
                    PrimaryColor = oi.ClothingItem.PrimaryColor,
                    SecondaryColor = oi.ClothingItem.SecondaryColor
                }
            }).ToList(),
            
            PlannerEntries = entity.PlannerEntries?.Select(pe => new PlannerEntry
            {
                Id = pe.Id,
                Date = pe.Date,
                Title = pe.Title,
                Comment = pe.Comment,
                Time = pe.Time,
                UserId = pe.UserId,
                OutfitId = pe.OutfitId
            }).ToList(),
            
            Favourites = entity.Favourites?.Select(f => new Favourite
            {
                Id = f.Id,
                UserId = f.UserId,
                OutfitId = f.OutfitId
            }).ToList(),
            
            IsFavourite = false
        };

        return result;
    }

    public DAL.DTO.Outfit? Map(Outfit? entity)
    {
        if (entity == null) return null;

        var result = new APP.DAL.DTO.Outfit
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Season = entity.Season,
            Style = entity.Style,
            UserId = entity.UserId
        };

        return result;
    }
}