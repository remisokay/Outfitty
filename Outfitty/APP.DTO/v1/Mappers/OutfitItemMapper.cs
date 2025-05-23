using BASE.Contracts;

namespace APP.DTO.v1.Mappers;

public class OutfitItemMapper : IMapper<OutfitItem, APP.BLL.DTO.OutfitItem>
{
    public OutfitItem? Map(BLL.DTO.OutfitItem? entity)
    {
        if (entity == null) return null;
        
        var result = new OutfitItem()
        {
            Id = entity.Id,
            OutfitId = entity.OutfitId,
            ClothingItemId = entity.ClothingItemId,
            DisplayOrder = entity.DisplayOrder,
            
            // business logic properties
            ClothingItemName = entity.ClothingItem?.Name,
            ImageUrl = entity.ImageUrl,
            ColorName = entity.ColorName
        };
        
        return result;
    }

    public BLL.DTO.OutfitItem? Map(OutfitItem? entity)
    {
        if (entity == null) return null;
        
        var result = new APP.BLL.DTO.OutfitItem()
        {
            Id = entity.Id,
            OutfitId = entity.OutfitId,
            ClothingItemId = entity.ClothingItemId,
            DisplayOrder = entity.DisplayOrder
        };
        
        return result;
    }
    
    public APP.BLL.DTO.OutfitItem Map(OutfitItemCreate entity)
    {
        var result = new APP.BLL.DTO.OutfitItem()
        {
            Id = Guid.NewGuid(),
            OutfitId = entity.OutfitId,
            ClothingItemId = entity.ClothingItemId,
            DisplayOrder = entity.DisplayOrder
        };
        
        return result;
    }
}