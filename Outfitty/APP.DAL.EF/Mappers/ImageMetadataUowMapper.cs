using APP.DAL.DTO;
using BASE.Contracts;
using Domain.identity;

namespace APP.DAL.EF.Mappers;

public class ImageMetadataUowMapper : IMapper<ImageMetadata, Domain.ImageMetadata>
{
    public ImageMetadata? Map(Domain.ImageMetadata? entity)
    {
        if (entity == null) return null;

        var result = new ImageMetadata()
        {
            Id = entity.Id,
            OriginalFileName = entity.OriginalFileName,
            ContentType = entity.ContentType,
            FileSize = entity.FileSize,
            Width = entity.Width,
            Height = entity.Height,
            StoragePath = entity.StoragePath,
            PublicUrl = entity.PublicUrl,
            ClothingItemId = entity.ClothingItemId,
            UserId = entity.UserId,
            ClothingItem = entity.ClothingItem == null ? null : new ClothingItem()
            {
                Id = entity.ClothingItem.Id,
                Name = entity.ClothingItem.Name,
                Type = entity.ClothingItem.Type,
                Style = entity.ClothingItem.Style,
                Season = entity.ClothingItem.Season,
                PrimaryColor = entity.ClothingItem.PrimaryColor,
                SecondaryColor = entity.ClothingItem.SecondaryColor,
                WardrobeId = entity.ClothingItem.WardrobeId
            },
            User = entity.User == null ? null : new AppUser()
            {
                Id = entity.User.Id,
                Username = entity.User.Username
            }
        };

        return result;
    }

    public Domain.ImageMetadata? Map(ImageMetadata? entity)
    {
        if (entity == null) return null;

        var result = new Domain.ImageMetadata()
        {
            Id = entity.Id,
            OriginalFileName = entity.OriginalFileName,
            ContentType = entity.ContentType,
            FileSize = entity.FileSize,
            Width = entity.Width,
            Height = entity.Height,
            StoragePath = entity.StoragePath,
            PublicUrl = entity.PublicUrl,
            ClothingItemId = entity.ClothingItemId,
            UserId = entity.UserId,
            ClothingItem = entity.ClothingItem == null ? null : new Domain.ClothingItem()
            {
                Id = entity.ClothingItem.Id,
                Name = entity.ClothingItem.Name,
                Type = entity.ClothingItem.Type,
                Style = entity.ClothingItem.Style,
                Season = entity.ClothingItem.Season,
                PrimaryColor = entity.ClothingItem.PrimaryColor,
                SecondaryColor = entity.ClothingItem.SecondaryColor,
                WardrobeId = entity.ClothingItem.WardrobeId
            },
            User = entity.User == null ? null : new AppUser()
            {
                Id = entity.User.Id,
                Username = entity.User.Username
            }
        };

        return result;
    }
}