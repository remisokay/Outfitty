using BASE.Contracts;

namespace APP.DTO.v1;

public class ImageMetadata : IDomainId
{
    public Guid Id { get; set; }
    
    public string OriginalFileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long FileSize { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string? PublicUrl { get; set; }
    
    public Guid? ClothingItemId { get; set; }
    public Guid? UserId { get; set; }
    
    public string FormattedFileSize { get; set; } = default!;
    public string Dimensions { get; set; } = default!;
    public bool IsProfileImage { get; set; }
}