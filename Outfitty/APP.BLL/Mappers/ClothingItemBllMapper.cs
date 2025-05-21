using APP.BLL.DTO;
using BASE.Contracts;

namespace APP.BLL.Mappers;

public class ClothingItemBllMapper : IMapper<ClothingItem, APP.DAL.DTO.ClothingItem>
{
    public ClothingItem? Map(DAL.DTO.ClothingItem? entity)
    {
        if (entity == null) return null;

        var result = new ClothingItem
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
            
            Wardrobe = entity.Wardrobe == null ? null : new Wardrobe
            {
                Id = entity.Wardrobe.Id,
                Name = entity.Wardrobe.Name,
                Description = entity.Wardrobe.Description,
                UserId = entity.Wardrobe.UserId
            },
            
            ImageMetadata = entity.ImageMetadata == null ? null : new ImageMetadata
            {
                Id = entity.ImageMetadata.Id,
                OriginalFileName = entity.ImageMetadata.OriginalFileName,
                ContentType = entity.ImageMetadata.ContentType,
                FileSize = entity.ImageMetadata.FileSize,
                Width = entity.ImageMetadata.Width,
                Height = entity.ImageMetadata.Height,
                StoragePath = entity.ImageMetadata.StoragePath,
                PublicUrl = entity.ImageMetadata.PublicUrl
            },
            
            OutfitCount = 0
        };

        return result;
    }

    public DAL.DTO.ClothingItem? Map(ClothingItem? entity)
    {
        if (entity == null) return null;
        
        var result = new APP.DAL.DTO.ClothingItem
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
}