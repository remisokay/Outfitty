using System.ComponentModel.DataAnnotations;

namespace Domain;

public class ClothesItemType : BaseEntity
{
    [MaxLength(128)]
    public string Type { get; set; } = default!; // top, bottom, dress ...
    
    [MaxLength(254)]
    public string? Description { get; set; }
}