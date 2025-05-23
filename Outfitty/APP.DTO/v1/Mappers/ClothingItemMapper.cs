using BASE.Contracts;

namespace APP.DTO.v1.Mappers;

public class ClothingItemMapper : IMapper<ClothingItem, APP.BLL.DTO.ClothingItem>
{
    public ClothingItem? Map(BLL.DTO.ClothingItem? entity)
    {
        if (entity == null) return null;
        
        var result = new ClothingItem()
        {
            Id = entity.Id,
            Name = entity.Name,
            Type = entity.Type,
            Style = entity.Style,
            Season = entity.Season,
            PrimaryColor = entity.PrimaryColor,
            SecondaryColor = entity.SecondaryColor,
            WardrobeId = entity.WardrobeId,
            ImageMetadataId = entity.ImageMetadataId,
            
            // business logic properties
            ImageUrl = entity.ImageMetadata?.PublicUrl,
            WardrobeName = entity.Wardrobe?.Name
        };
        
        return result;
    }

    public BLL.DTO.ClothingItem? Map(ClothingItem? entity)
    {
        if (entity == null) return null;
        
        var result = new APP.BLL.DTO.ClothingItem()
        {
            Id = entity.Id,
            Name = entity.Name,
            Type = entity.Type,
            Style = entity.Style,
            Season = entity.Season,
            PrimaryColor = entity.PrimaryColor,
            SecondaryColor = entity.SecondaryColor,
            WardrobeId = entity.WardrobeId,
            ImageMetadataId = entity.ImageMetadataId
        };
        
        return result;
    }

    public APP.BLL.DTO.ClothingItem Map(ClothingItemCreate entity)
    {
        var result = new APP.BLL.DTO.ClothingItem()
        {
            Id = Guid.NewGuid(),
            Name = entity.Name,
            Type = entity.Type,
            Style = entity.Style,
            Season = entity.Season,
            PrimaryColor = entity.PrimaryColor,
            SecondaryColor = entity.SecondaryColor,
            WardrobeId = entity.WardrobeId,
            ImageMetadataId = entity.ImageMetadataId
        };
        
        return result;
    }
}