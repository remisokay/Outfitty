using BASE.Contracts;

namespace APP.DTO.v1.Mappers;

public class OutfitMapper : IMapper<Outfit, APP.BLL.DTO.Outfit>
{
    private readonly OutfitItemMapper _outfitItemMapper;
    
    public OutfitMapper()
    {
        _outfitItemMapper = new OutfitItemMapper();
    }
    
    public Outfit? Map(BLL.DTO.Outfit? entity)
    {
        if (entity == null) return null;
        
        var result = new Outfit()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Season = entity.Season,
            Style = entity.Style,
            UserId = entity.UserId,
            
            // business logic properties
            IsFavorite = entity.IsFavourite,
            ItemCount = entity.OutfitItems?.Count ?? 0,
            OutfitItems = entity.OutfitItems?.Select(oi => _outfitItemMapper.Map(oi)!).ToList()
        };
        
        return result;
    }

    public BLL.DTO.Outfit? Map(Outfit? entity)
    {
        if (entity == null) return null;
        
        var result = new APP.BLL.DTO.Outfit()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Season = entity.Season,
            Style = entity.Style,
            UserId = entity.UserId,
            IsFavourite = entity.IsFavorite
        };
        
        return result;
    }
    
    public APP.BLL.DTO.Outfit Map(OutfitCreate entity)
    {
        var result = new APP.BLL.DTO.Outfit()
        {
            Id = Guid.NewGuid(),
            Name = entity.Name,
            Description = entity.Description,
            Season = entity.Season,
            Style = entity.Style,
            UserId = entity.UserId,
            IsFavourite = false
        };
        
        return result;
    }
}