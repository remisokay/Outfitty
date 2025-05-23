using BASE.Contracts;

namespace APP.DTO.v1;

public class OutfitItem : IDomainId
{
    public Guid Id { get; set; }
    
    public Guid OutfitId { get; set; }
    public Guid ClothingItemId { get; set; }
    public int DisplayOrder { get; set; }
    
    public string? ClothingItemName { get; set; }
    public string? ImageUrl { get; set; }
    public string? ColorName { get; set; }
}