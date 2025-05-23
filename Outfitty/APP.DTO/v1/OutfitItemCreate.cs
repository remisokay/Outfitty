namespace APP.DTO.v1;

public class OutfitItemCreate
{
    public Guid OutfitId { get; set; }
    public Guid ClothingItemId { get; set; }
    public int DisplayOrder { get; set; }
}