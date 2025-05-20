using BASE.BLL.Contracts;

namespace APP.BLL.Contracts;

public interface IImageMetadataService : IBaseService<DTO.ImageMetadata>
{
    // Image management
    Task<DTO.ImageMetadata> UploadImageAsync(Stream imageStream, string fileName, string contentType);
    Task<Stream?> GetImageDataAsync(Guid imageId);
    Task<string?> GetPublicUrlAsync(Guid imageId);
    
    // User-specific operations
    Task<IEnumerable<DTO.ImageMetadata>> GetUserImagesAsync(Guid userId);
    Task<DTO.ImageMetadata?> GetProfileImageAsync(Guid userId);
    Task SetProfileImageAsync(Guid userId, Guid imageId);
    
    // Clothing-specific operations
    Task<DTO.ImageMetadata?> GetClothingItemImageAsync(Guid clothingItemId);
    Task<IEnumerable<DTO.ImageMetadata>> GetImagesForWardrobeAsync(Guid wardrobeId);
}