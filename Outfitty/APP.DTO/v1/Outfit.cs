using System.ComponentModel.DataAnnotations;
using APP.BLL.DTO;
using BASE.Contracts;
using Domain.Enums;

namespace APP.DTO.v1;

public class Outfit : IDomainId
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public ClothingSeason Season { get; set; }
    public ClothingStyle Style { get; set; }
    
    public Guid UserId { get; set; }
    
    public bool IsFavorite { get; set; }
    public int ItemCount { get; set; }
    public List<OutfitItem>? OutfitItems { get; set; }
}