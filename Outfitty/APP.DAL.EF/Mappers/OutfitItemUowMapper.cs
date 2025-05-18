using APP.DAL.DTO;
using BASE.Contracts;

namespace APP.DAL.EF.Mappers;

public class OutfitItemUowMapper : IMapper<OutfitItem, Domain.OutfitItem>
{
    public OutfitItem? Map(Domain.OutfitItem? entity)
    {
        if (entity == null) return null;

        var result = new OutfitItem()
        {
            Id = entity.Id,
            OutfitId = entity.OutfitId,
            DisplayOrder = entity.DisplayOrder,
            ClothingItemId = entity.ClothingItemId,
            Outfit = entity.Outfit == null ? null : new Outfit()
            {
                Id = entity.Outfit.Id,
                Name = entity.Outfit.Name,
                Description = entity.Outfit.Description,
                Season = entity.Outfit.Season,
                Style = entity.Outfit.Style,
                UserId = entity.Outfit.UserId
            },
            ClothingItem = entity.ClothingItem == null ? null : new ClothingItem()
            {
                Id = entity.ClothingItem.Id,
                Name = entity.ClothingItem.Name,
                Type = entity.ClothingItem.Type,
                Style = entity.ClothingItem.Style,
                Season = entity.ClothingItem.Season,
                PrimaryColor = entity.ClothingItem.PrimaryColor,
                SecondaryColor = entity.ClothingItem.SecondaryColor,
                WardrobeId = entity.ClothingItem.WardrobeId,
                ImageMetadataId = entity.ClothingItem.ImageMetadataId
            }
        };
        return result;
    }

    public Domain.OutfitItem? Map(OutfitItem? entity)
    {
        if (entity == null) return null;

        var result = new Domain.OutfitItem()
        {
            Id = entity.Id,
            OutfitId = entity.OutfitId,
            DisplayOrder = entity.DisplayOrder,
            ClothingItemId = entity.ClothingItemId,
            Outfit = entity.Outfit == null ? null : new Domain.Outfit()
            {
                Id = entity.Outfit.Id,
                Name = entity.Outfit.Name,
                Description = entity.Outfit.Description,
                Season = entity.Outfit.Season,
                Style = entity.Outfit.Style,
                UserId = entity.Outfit.UserId
            },
            ClothingItem = entity.ClothingItem == null ? null : new Domain.ClothingItem()
            {
                Id = entity.ClothingItem.Id,
                Name = entity.ClothingItem.Name,
                Type = entity.ClothingItem.Type,
                Style = entity.ClothingItem.Style,
                Season = entity.ClothingItem.Season,
                PrimaryColor = entity.ClothingItem.PrimaryColor,
                SecondaryColor = entity.ClothingItem.SecondaryColor,
                WardrobeId = entity.ClothingItem.WardrobeId,
                ImageMetadataId = entity.ClothingItem.ImageMetadataId
            }
        };
        
        return result;
    }
}