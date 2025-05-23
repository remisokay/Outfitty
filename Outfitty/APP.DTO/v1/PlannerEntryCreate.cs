using System.ComponentModel.DataAnnotations;

namespace APP.DTO.v1;

public class PlannerEntryCreate
{
    public DateTime Date { get; set; }
    
    [Required]
    [MaxLength(128)]
    public string Title { get; set; } = default!;
    
    [MaxLength(500)]
    public string? Comment { get; set; }
    
    public TimeSpan? Time { get; set; }
    
    public Guid UserId { get; set; }
    public Guid OutfitId { get; set; }
}