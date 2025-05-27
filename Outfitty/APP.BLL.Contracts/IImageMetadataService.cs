using BASE.BLL.Contracts;

namespace APP.BLL.Contracts;

public interface IImageMetadataService : IBaseService<DTO.ImageMetadata>
{
    // Image management
    Task<DTO.ImageMetadata> UploadImageAsync(Stream imageStream, string fileName, string contentType);
    Task<Stream?> GetImageDataAsync(Guid imageId);
    Task<string?> GetPublicUrlAsync(Guid imageId);
    Task<bool> DeleteImageAsync(Guid imageId, Guid userId);
    
    // User-specific operations
    Task<IEnumerable<DTO.ImageMetadata>> GetUserImagesAsync(Guid userId);
    Task<DTO.ImageMetadata?> GetProfileImageAsync(Guid userId);
    Task SetProfileImageAsync(Guid userId, Guid imageId);
    
    // Clothing-specific operations
    Task<DTO.ImageMetadata?> GetClothingItemImageAsync(Guid clothingItemId);
    Task<IEnumerable<DTO.ImageMetadata>> GetImagesForWardrobeAsync(Guid wardrobeId);
    Task<DTO.ImageMetadata> AssignImageToClothingItemAsync(Guid imageId, Guid clothingItemId, Guid userId);
    
    // Outfit-specific operations
    Task<DTO.ImageMetadata?> GetOutfitImageAsync(Guid outfitId);
    Task<DTO.ImageMetadata> AssignImageToOutfitAsync(Guid imageId, Guid outfitId, Guid userId);
    
}