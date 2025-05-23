using System.ComponentModel.DataAnnotations;
using BASE.Contracts;
using Domain.identity;

namespace APP.DTO.v1;

public class Wardrobe : IDomainId
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public Guid UserId { get; set; }
    
    public int ItemCount { get; set; }
    public AppUser? Username { get; set; }
}