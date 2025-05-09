using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Wardrobe : BaseEntity
{
    [MaxLength(128)]
    public string Name { get; set; } = default!; // Holiday wardrobe
    [MaxLength(254)]
    public string? Description { get; set; }
}