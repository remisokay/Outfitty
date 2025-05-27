using APP.BLL.Contracts;
using APP.BLL.DTO;
using APP.DAL.Contracts;
using BASE.BLL;
using BASE.Contracts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;


namespace APP.BLL.Services;

public class ImageMetadataService : BaseService<ImageMetadata, APP.DAL.DTO.ImageMetadata, IImageMetadataRepository>, IImageMetadataService
{
    private readonly IAppUow _uow;
    private readonly string _storageBasePath;
    private readonly string _publicUrlBase;
    
    public ImageMetadataService(
        IAppUow serviceUow,
        IMapper<ImageMetadata, APP.DAL.DTO.ImageMetadata> mapper)
        : base(serviceUow, serviceUow.ImageMetadataRepository, mapper)
    {
        _uow = serviceUow;
        // TODO: These should come from configuration in a real application
        _storageBasePath = Path.Combine("wwwroot", "uploads", "images");
        _publicUrlBase = "/uploads/images/";
        
        Directory.CreateDirectory(_storageBasePath);
    }

    public async Task<ImageMetadata> UploadImageAsync(Stream imageStream, string fileName, string contentType)
    {
        // validation
        if (imageStream == null || imageStream.Length == 0)
            throw new ArgumentException("image stream cannot be null or empty");
        
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Filename cannot be null or empty");
        
        if (!contentType.StartsWith("image/"))
            throw new ArgumentException("File must be an image");
        
        // image dimensions
        int width, height;
        IImageFormat imageFormat;
        
        try
        {
            imageStream.Position = 0;
            var imageInfo = await Image.IdentifyAsync(imageStream);
        
            if (imageInfo == null)
                throw new ArgumentException("Invalid image file - cannot read image data");
        
            width = imageInfo.Width;
            height = imageInfo.Height;
        
            imageFormat = imageInfo.Metadata.DecodedImageFormat 
                          ?? throw new ArgumentException("Could not determine image format");
            
            contentType = GetContentTypeFromFormat(imageFormat);
        }
        catch (InvalidImageContentException ex)
        {
            throw new ArgumentException($"Invalid image file: {ex.Message}", ex);
        }
        catch (UnknownImageFormatException ex)
        {
            throw new ArgumentException($"Unsupported image format: {ex.Message}", ex);
        }
        
        // generating unique filename
        var fileExtension = GetFileExtensionFromFormat(imageFormat);
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var storagePath = Path.Combine(_storageBasePath, uniqueFileName);
        var publicUrl = $"{_publicUrlBase}{uniqueFileName}";
        
        // saving file to storage
        imageStream.Position = 0;
        await using (var fileStream = new FileStream(storagePath, FileMode.Create))
        {
            await imageStream.CopyToAsync(fileStream);
        }
        
        // create
        var imageMetadata = new APP.DAL.DTO.ImageMetadata
        {
            Id = Guid.NewGuid(),
            OriginalFileName = fileName,
            ContentType = contentType,
            FileSize = imageStream.Length,
            Width = width,
            Height = height,
            StoragePath = storagePath,
            PublicUrl = publicUrl
        };
        
        // save to database
        ServiceRepository.Add(imageMetadata);
        await _uow.SaveChangesAsync();
        
        return Mapper.Map(imageMetadata)!;

    }

    public async Task<Stream?> GetImageDataAsync(Guid imageId)
    {
        var dalImage = await ServiceRepository.FindAsync(imageId);
        if (dalImage == null || !File.Exists(dalImage.StoragePath))
            return null;
        return new FileStream(dalImage.StoragePath, FileMode.Open, FileAccess.Read);
    }

    public async Task<string?> GetPublicUrlAsync(Guid imageId)
    {
        var dalImage = await ServiceRepository.FindAsync(imageId);
        return dalImage?.PublicUrl;
    }

    public async Task<IEnumerable<ImageMetadata>> GetUserImagesAsync(Guid userId)
    {
        var dalImages = await ServiceRepository.GetImagesByUserAsync(userId);
        return dalImages.Select(img => Mapper.Map(img)!).ToList();
    }

    public async Task<ImageMetadata?> GetProfileImageAsync(Guid userId)
    {
        var dalImage = await ServiceRepository.GetImageByUserProfileAsync(userId);
        if (dalImage == null) return null;
        
        var bllImage = Mapper.Map(dalImage);
        return bllImage;
    }

    public async Task SetProfileImageAsync(Guid userId, Guid imageId)
    {
        var dalImage = await ServiceRepository.FindAsync(imageId, userId);
        if (dalImage == null)
            throw new ArgumentException("Image not found");
        
        if (dalImage.ClothingItemId.HasValue)
            throw new ArgumentException("Image is already a clothing item, choose a normal profile pic!");
        
        dalImage.UserId = userId;
        dalImage.ClothingItemId = null;
        
        await ServiceRepository.UpdateAsync(dalImage);
        await _uow.SaveChangesAsync();
        
        // TODO: if I have time, update the user's ProfileImageId
    }

    public async Task<ImageMetadata?> GetClothingItemImageAsync(Guid clothingItemId)
    {
        var clothingItem = await _uow.ClothingItemRepository.FindAsync(clothingItemId);
        if (clothingItem == null) return null;
        
        var dalImage = await ServiceRepository.GetImageByClothingItemAsync(clothingItemId, clothingItem.Wardrobe?.UserId ?? Guid.Empty);
        if (dalImage == null) return null;
        
        return Mapper.Map(dalImage);
    }

    public async Task<IEnumerable<ImageMetadata>> GetImagesForWardrobeAsync(Guid wardrobeId)
    {
        var wardrobe = await _uow.WardrobeRepository.FindAsync(wardrobeId);
        if (wardrobe == null) return Enumerable.Empty<ImageMetadata>();
        
        var clothingItems = await _uow.ClothingItemRepository.GetClothingItemsByWardrobeAsync(wardrobeId, wardrobe.UserId);
        
        var imageList = new List<APP.DAL.DTO.ImageMetadata>();
        foreach (var item in clothingItems)
        {
            var image = await ServiceRepository.GetImageByClothingItemAsync(item.Id, wardrobe.UserId);
            if (image != null) imageList.Add(image);
        }
        return imageList.Select(img => Mapper.Map(img)!).ToList();
    }

    public async Task<ImageMetadata> AssignImageToClothingItemAsync(Guid imageId, Guid clothingItemId, Guid userId)
    {
        var dalImage = await ServiceRepository.FindAsync(imageId, userId);
        if (dalImage == null) throw new ArgumentException("Image not found");
        
        var clothingItem = await _uow.ClothingItemRepository.FindAsync(clothingItemId);
        if (clothingItem?.Wardrobe?.UserId != userId) throw new ArgumentException("Clothing item not found");
        
        // if image is already assigned to another item
        if (dalImage.ClothingItemId.HasValue && dalImage.ClothingItemId != clothingItemId)
        {
            throw new InvalidOperationException("Image is already assigned to a clothing item");
        }
        
        // assign the image
        dalImage.ClothingItemId = clothingItemId;
        dalImage.UserId = userId;
        clothingItem.ImageMetadataId = imageId;

        await ServiceRepository.UpdateAsync(dalImage);
        await _uow.SaveChangesAsync();
        
        return Mapper.Map(dalImage)!;
    }

    public async Task<ImageMetadata?> GetOutfitImageAsync(Guid outfitId)
    {
        var outfit = await _uow.OutfitRepository.FindAsync(outfitId);
        if (outfit == null) return null;
        
        var dalImage = await ServiceRepository.GetImageByOutfitAsync(outfitId, outfit?.UserId ?? Guid.Empty);
        if (dalImage == null) return null;
        
        return Mapper.Map(dalImage);
    }

    public async Task<ImageMetadata> AssignImageToOutfitAsync(Guid imageId, Guid outfitId, Guid userId)
    {
        var dalImage = await ServiceRepository.FindAsync(imageId, userId);
        if (dalImage == null) throw new ArgumentException("Image not found");
        
        var outfit = await _uow.OutfitRepository.FindAsync(outfitId, userId);
        if (outfit?.UserId != userId) throw new ArgumentException("Outfit not found");
        
        // if image is already assigned to another item
        if (dalImage.OutfitId.HasValue && dalImage.OutfitId != outfitId)
        {
            throw new InvalidOperationException("Image is already assigned to an outfit");
        }
        
        // assign the image
        dalImage.OutfitId = outfitId;
        dalImage.UserId = userId;
        outfit.ImageMetadataId = imageId;

        await ServiceRepository.UpdateAsync(dalImage);
        await _uow.SaveChangesAsync();
        
        return Mapper.Map(dalImage)!;
    }

    public async Task<bool> DeleteImageAsync(Guid imageId, Guid userId)
    {
        var dalImage = await ServiceRepository.FindAsync(imageId, userId);
        if (dalImage == null) return false;
        
        if (dalImage.ClothingItemId.HasValue)
            throw new InvalidOperationException("Delete failed.");
        
        if (File.Exists(dalImage.StoragePath))
        {
            try
            {
                File.Delete(dalImage.StoragePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete file {dalImage.StoragePath}: {ex.Message}");
            }
        }
        
        var success = await ServiceRepository.DeleteImageAsync(imageId, userId);
        if (success)
        {
            await _uow.SaveChangesAsync();
        }

        return success;
    }
    
    
    // helper methods
    private static string GetContentTypeFromFormat(IImageFormat format)
    {
        return format.Name.ToLowerInvariant() switch
        {
            "jpeg" => "image/jpeg",
            "png" => "image/png",
            "gif" => "image/gif",
            "bmp" => "image/bmp",
            "webp" => "image/webp",
            "tiff" => "image/tiff",
            _ => "image/jpeg" // default fallback
        };
    }
    
    private static string GetFileExtensionFromFormat(IImageFormat format)
    {
        return format.Name.ToLowerInvariant() switch
        {
            "jpeg" => ".jpg",
            "png" => ".png",
            "gif" => ".gif",
            "bmp" => ".bmp",
            "webp" => ".webp",
            "tiff" => ".tiff",
            _ => ".jpg" // default fallback
        };
    }
    
}