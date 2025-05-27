using BASE.DAL.Contracts;

namespace APP.DAL.Contracts;

public interface IImageMetadataRepository : IBaseRepository<DTO.ImageMetadata>, IImageMetadataRepositoryCustom
{
    
}

public interface IImageMetadataRepositoryCustom
{
    Task<DTO.ImageMetadata?> GetImageByClothingItemAsync(Guid clothingItemId, Guid userId);
    Task<DTO.ImageMetadata?> GetImageByOutfitAsync(Guid outfitId, Guid userId);
    Task<DTO.ImageMetadata?> GetImageByUserProfileAsync(Guid userId);
    Task<IEnumerable<DTO.ImageMetadata>> GetImagesByUserAsync(Guid userId);
    Task<bool> DeleteImageAsync(Guid imageId, Guid userId);
}