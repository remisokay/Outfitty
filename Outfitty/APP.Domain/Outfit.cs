using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Outfit : BaseEntity
{
    [MaxLength(128)]
    public string Name { get; set; } = default!; // school look
    [MaxLength(128)]
    public string Category { get; set; } = default!; // casual
    [MaxLength(128)]
    public string? Comment { get; set; } // good to wear on colder days
    
    //FK
    public Guid UserId { get; set; }
    public User? User { get; set; }
    
    public ICollection<ClothesItem> ClothesItems { get; set; } = default!; //?
    
    
    
}