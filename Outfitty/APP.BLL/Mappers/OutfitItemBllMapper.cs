using APP.BLL.DTO;
using BASE.Contracts;

namespace APP.BLL.Mappers;

public class OutfitItemBllMapper : IMapper<OutfitItem, APP.DAL.DTO.OutfitItem>
{
    public OutfitItem? Map(DAL.DTO.OutfitItem? entity)
    {
        if (entity == null) return null;

        var result = new OutfitItem
        {
            Id = entity.Id,
            OutfitId = entity.OutfitId,
            DisplayOrder = entity.DisplayOrder,
            ClothingItemId = entity.ClothingItemId,
            
            Outfit = entity.Outfit == null ? null : new Outfit
            {
                Id = entity.Outfit.Id,
                Name = entity.Outfit.Name,
                Description = entity.Outfit.Description,
                Season = entity.Outfit.Season,
                Style = entity.Outfit.Style,
                UserId = entity.Outfit.UserId
            },
            
            ClothingItem = entity.ClothingItem == null ? null : new ClothingItem
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

    public DAL.DTO.OutfitItem? Map(OutfitItem? entity)
    {
        if (entity == null) return null;

        var result = new APP.DAL.DTO.OutfitItem
        {
            Id = entity.Id,
            OutfitId = entity.OutfitId,
            DisplayOrder = entity.DisplayOrder,
            ClothingItemId = entity.ClothingItemId
        };

        return result;
    }
}