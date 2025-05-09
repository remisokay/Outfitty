using BASE.Domain;
using Domain.identity;

namespace Domain;

public class ImageMetadata : BaseEntity
{
    public string OriginalFileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long FileSize { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string StoragePath { get; set; } = default!;
    public string? PublicUrl { get; set; }
    
    //FK
    public Guid? ClothingItemId { get; set; }
    public virtual ClothingItem? ClothingItem { get; set; }
    
    public Guid? UserId { get; set; }
    public virtual AppUser? User { get; set; }
}