using APP.BLL.DTO;
using BASE.Contracts;
using Domain.identity;

namespace APP.BLL.Mappers;

public class ImageMetadataBllMapper : IMapper<ImageMetadata, APP.DAL.DTO.ImageMetadata>
{
    public ImageMetadata? Map(DAL.DTO.ImageMetadata? entity)
    {
        if (entity == null) return null;

        var result = new ImageMetadata
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
            OutfitId = entity.OutfitId,
            UserId = entity.UserId,
            
            ClothingItem = entity.ClothingItem == null ? null : new ClothingItem
            {
                Id = entity.ClothingItem.Id,
                Name = entity.ClothingItem.Name,
                Type = entity.ClothingItem.Type,
                Style = entity.ClothingItem.Style,
                Season = entity.ClothingItem.Season,
                PrimaryColor = entity.ClothingItem.PrimaryColor,
                SecondaryColor = entity.ClothingItem.SecondaryColor
            },
            
            Outfit = entity.Outfit == null ? null : new Outfit
            {
                Id = entity.Outfit.Id,
                Name = entity.Outfit.Name,
                Description = entity.Outfit.Description,
                Season = entity.Outfit.Season,
                Style = entity.Outfit.Style,
                UserId = entity.Outfit.UserId
            },
            
            User = entity.User == null ? null : new AppUser
            {
                Id = entity.User.Id,
                Username = entity.User.Username
            },
        };

        return result;
    }

    public DAL.DTO.ImageMetadata? Map(ImageMetadata? entity)
    {
        if (entity == null) return null;

        var result = new APP.DAL.DTO.ImageMetadata
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
            OutfitId = entity.OutfitId,
            UserId = entity.UserId
        };

        return result;
    }
}