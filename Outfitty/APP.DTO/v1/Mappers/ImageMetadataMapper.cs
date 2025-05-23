using BASE.Contracts;

namespace APP.DTO.v1.Mappers;

public class ImageMetadataMapper : IMapper<ImageMetadata, APP.BLL.DTO.ImageMetadata>
{
    public ImageMetadata? Map(BLL.DTO.ImageMetadata? entity)
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
            PublicUrl = entity.PublicUrl,
            ClothingItemId = entity.ClothingItemId,
            UserId = entity.UserId,
            
            // business logic properties
            FormattedFileSize = entity.FormattedFileSize,
            Dimensions = entity.Dimensions,
            IsProfileImage = entity.IsProfileImage
        };
        
        return result;
    }

    public BLL.DTO.ImageMetadata? Map(ImageMetadata? entity)
    {
        if (entity == null) return null;
        
        var result = new APP.BLL.DTO.ImageMetadata()
        {
            Id = entity.Id,
            OriginalFileName = entity.OriginalFileName,
            ContentType = entity.ContentType,
            FileSize = entity.FileSize,
            Width = entity.Width,
            Height = entity.Height,
            PublicUrl = entity.PublicUrl,
            ClothingItemId = entity.ClothingItemId,
            UserId = entity.UserId
        };
        
        return result;
    }
    
    public APP.BLL.DTO.ImageMetadata Map(ImageMetadataCreate entity)
    {
        var result = new APP.BLL.DTO.ImageMetadata()
        {
            Id = Guid.NewGuid(),
            OriginalFileName = entity.OriginalFileName,
            ContentType = entity.ContentType,
            FileSize = entity.FileSize,
            Width = entity.Width,
            Height = entity.Height,
            ClothingItemId = entity.ClothingItemId,
            UserId = entity.UserId
        };
        
        return result;
    }
}