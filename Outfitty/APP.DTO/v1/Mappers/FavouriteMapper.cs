using BASE.Contracts;

namespace APP.DTO.v1.Mappers;

public class FavouriteMapper : IMapper<Favourite, APP.BLL.DTO.Favourite>
{
    public Favourite? Map(BLL.DTO.Favourite? entity)
    {
        if (entity == null) return null;
        
        var result = new Favourite()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            OutfitId = entity.OutfitId,
            
            // business logic properties
            OutfitName = entity.OutfitName,
            OutfitStyle = entity.OutfitStyle?.ToString(),
            OutfitSeason = entity.OutfitSeason?.ToString()
        };
        
        return result;
    }

    public BLL.DTO.Favourite? Map(Favourite? entity)
    {
        if (entity == null) return null;
        
        var result = new APP.BLL.DTO.Favourite()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            OutfitId = entity.OutfitId
        };
        
        return result;
    }
    
    public APP.BLL.DTO.Favourite Map(FavouriteCreate entity)
    {
        var result = new APP.BLL.DTO.Favourite()
        {
            Id = Guid.NewGuid(),
            UserId = entity.UserId,
            OutfitId = entity.OutfitId
        };
        
        return result;
    }
}