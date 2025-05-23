using BASE.Contracts;

namespace APP.DTO.v1.Mappers;

public class WardrobeMapper : IMapper<Wardrobe, APP.BLL.DTO.Wardrobe>
{
    public Wardrobe? Map(BLL.DTO.Wardrobe? entity)
    {
        if (entity == null) return null;
        
        var result = new Wardrobe()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            UserId = entity.UserId,
            
            // business logic properties
            ItemCount = entity.ItemCount,
            Username = entity.User
        };
        
        return result;
    }

    public BLL.DTO.Wardrobe? Map(Wardrobe? entity)
    {
        if (entity == null) return null;
        
        var result = new APP.BLL.DTO.Wardrobe()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            UserId = entity.UserId
        };
        
        return result;
    }
    
    public APP.BLL.DTO.Wardrobe Map(WardrobeCreate entity)
    {
        var result = new APP.BLL.DTO.Wardrobe()
        {
            Id = Guid.NewGuid(),
            Name = entity.Name,
            Description = entity.Description,
            UserId = entity.UserId
        };
        
        return result;
    }
}