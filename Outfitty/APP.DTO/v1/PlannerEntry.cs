using System.ComponentModel.DataAnnotations;
using BASE.Contracts;

namespace APP.DTO.v1;

public class PlannerEntry : IDomainId
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    
    [Required]
    [MaxLength(128)]
    public string Title { get; set; } = default!;
    
    [MaxLength(500)]
    public string? Comment { get; set; }
    
    public TimeSpan? Time { get; set; }
    
    public Guid UserId { get; set; }
    public Guid OutfitId { get; set; }
    
    public string? OutfitName { get; set; }
    public bool IsUpcoming { get; set; }
    public bool IsToday { get; set; }
}