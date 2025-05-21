using APP.BLL.Contracts;
using APP.BLL.DTO;
using APP.DAL.Contracts;
using BASE.BLL;
using BASE.Contracts;
using BASE.DAL.Contracts;

namespace APP.BLL.Services;

public class ImageMetadataService : BaseService<ImageMetadata, APP.DAL.DTO.ImageMetadata, APP.DAL.Contracts.IImageMetadataRepository>, IImageMetadataService
{
    public ImageMetadataService(
        IAppUow serviceUow,
        IMapper<ImageMetadata, APP.DAL.DTO.ImageMetadata> mapper)
        : base(serviceUow, serviceUow.ImageMetadataRepository, mapper)
    {
    }

    public Task<ImageMetadata> UploadImageAsync(Stream imageStream, string fileName, string contentType)
    {
        throw new NotImplementedException();
    }

    public Task<Stream?> GetImageDataAsync(Guid imageId)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetPublicUrlAsync(Guid imageId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ImageMetadata>> GetUserImagesAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<ImageMetadata?> GetProfileImageAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task SetProfileImageAsync(Guid userId, Guid imageId)
    {
        throw new NotImplementedException();
    }

    public Task<ImageMetadata?> GetClothingItemImageAsync(Guid clothingItemId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ImageMetadata>> GetImagesForWardrobeAsync(Guid wardrobeId)
    {
        throw new NotImplementedException();
    }
}