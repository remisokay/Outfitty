namespace APP.DTO.v1;

public class ImageMetadataCreate
{
    public string OriginalFileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long FileSize { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    
    public Guid? ClothingItemId { get; set; }
    public Guid? UserId { get; set; }
}