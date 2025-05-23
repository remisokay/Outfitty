using BASE.Contracts;

namespace APP.DTO.v1;

public class Favourite : IDomainId
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid OutfitId { get; set; }
    
    public string? OutfitName { get; set; }
    public string? OutfitStyle { get; set; }
    public string? OutfitSeason { get; set; }
}