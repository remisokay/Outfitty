using BASE.Contracts;
using Domain;
using Domain.identity;

namespace APP.BLL.DTO;

public class ImageMetadata : IDomainId
{
    public Guid Id { get; set; }
    
    // [Display(Name = nameof(OriginalFileName), ResourceType = typeof(APP.Resources.Domain.ImageMetadata))]
    public string OriginalFileName { get; set; } = default!;

    // [Display(Name = nameof(ContentType), ResourceType = typeof(APP.Resources.Domain.ImageMetadata))]
    public string ContentType { get; set; } = default!;

    // [Display(Name = nameof(FileSize), ResourceType = typeof(APP.Resources.Domain.ImageMetadata))]
    public long FileSize { get; set; }

    // [Display(Name = nameof(Width), ResourceType = typeof(APP.Resources.Domain.ImageMetadata))]
    public int Width { get; set; }

    // [Display(Name = nameof(Height), ResourceType = typeof(APP.Resources.Domain.ImageMetadata))]
    public int Height { get; set; }

    // [Display(Name = nameof(StoragePath), ResourceType = typeof(APP.Resources.Domain.ImageMetadata))]
    public string StoragePath { get; set; } = default!;

    // [Display(Name = nameof(PublicUrl), ResourceType = typeof(APP.Resources.Domain.ImageMetadata))]
    public string? PublicUrl { get; set; }

    // [Display(Name = nameof(ClothingItemId), ResourceType = typeof(APP.Resources.Domain.ImageMetadata))]
    public Guid? ClothingItemId { get; set; }

    // [Display(Name = nameof(ClothingItem), ResourceType = typeof(APP.Resources.Domain.ImageMetadata))]
    public ClothingItem? ClothingItem { get; set; }

    // [Display(Name = nameof(UserId), ResourceType = typeof(APP.Resources.Domain.ImageMetadata))]
    public Guid? UserId { get; set; }

    // [Display(Name = nameof(User), ResourceType = typeof(APP.Resources.Domain.ImageMetadata))]
    public AppUser? User { get; set; }
    
    // BLL functions
    public bool IsImage => ContentType.StartsWith("image/");
    public bool IsProfileImage => UserId.HasValue && !ClothingItemId.HasValue;
    public bool IsClothingImage => ClothingItemId.HasValue;
    
    public string FileExtension => 
        OriginalFileName.Contains('.') 
            ? OriginalFileName.Substring(OriginalFileName.LastIndexOf('.'))
            : string.Empty;
    
    public string FormattedFileSize
    {
        get
        {
            return FileSize switch
            {
                < 1024 => $"{FileSize} B",
                < 1024 * 1024 => $"{FileSize / 1024.0:F1} KB",
                _ => $"{FileSize / (1024.0 * 1024.0):F2} MB"
            };
        }
    }
    
    public string Dimensions => $"{Width} × {Height}";
}