using APP.DAL.DTO;
using BASE.Contracts;

namespace APP.DAL.EF.Mappers;

public class ClothingItemUowMapper : IMapper<ClothingItem, Domain.ClothingItem>
{
    public ClothingItem? Map(Domain.ClothingItem? entity)
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
            Wardrobe = entity.Wardrobe == null ? null : new Wardrobe()
            {
                Id = entity.WardrobeId,
                Name = entity.Wardrobe.Name,
                Description = entity.Wardrobe.Description,
                UserId = entity.Wardrobe.UserId,
            },
            ImageMetadataId = entity.ImageMetadataId,
            ImageMetadata = entity.ImageMetadata == null ? null : new ImageMetadata()
            {
                Id = entity.ImageMetadata.Id,
                OriginalFileName = entity.ImageMetadata.OriginalFileName,
                ContentType = entity.ImageMetadata.ContentType,
                FileSize = entity.ImageMetadata.FileSize,
                Width = entity.ImageMetadata.Width,
                Height = entity.ImageMetadata.Height,
                StoragePath = entity.ImageMetadata.StoragePath,
                PublicUrl = entity.ImageMetadata.PublicUrl,
                ClothingItemId = entity.ImageMetadata.ClothingItemId,
                UserId = entity.ImageMetadata.UserId
            }
        };
        return result;
    }

    public Domain.ClothingItem? Map(ClothingItem? entity)
    {
        if (entity == null) return null;

        var result = new Domain.ClothingItem()
        {
            Id = entity.Id,
            Name = entity.Name,
            Type = entity.Type,
            Style = entity.Style,
            Season = entity.Season,
            PrimaryColor = entity.PrimaryColor,
            SecondaryColor = entity.SecondaryColor,
            WardrobeId = entity.WardrobeId,
            Wardrobe = entity.Wardrobe == null ? null : new Domain.Wardrobe()
            {
                Id = entity.Wardrobe.Id,
                Name = entity.Wardrobe.Name,
                Description = entity.Wardrobe.Description,
                UserId = entity.Wardrobe.UserId
            },
            ImageMetadataId = entity.ImageMetadataId,
            ImageMetadata = entity.ImageMetadata == null ? null : new Domain.ImageMetadata()
            {
                Id = entity.ImageMetadata.Id,
                OriginalFileName = entity.ImageMetadata.OriginalFileName,
                ContentType = entity.ImageMetadata.ContentType,
                FileSize = entity.ImageMetadata.FileSize,
                Width = entity.ImageMetadata.Width,
                Height = entity.ImageMetadata.Height,
                StoragePath = entity.ImageMetadata.StoragePath,
                PublicUrl = entity.ImageMetadata.PublicUrl,
                ClothingItemId = entity.ImageMetadata.ClothingItemId,
                UserId = entity.ImageMetadata.UserId
            }
        };
        return result;
    }
}