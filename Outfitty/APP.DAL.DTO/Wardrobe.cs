using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.Arm;
using System.Security.Principal;
using BASE.Contracts;
using Domain;
using Domain.identity;

namespace APP.DAL.DTO;

public class Wardrobe : IDomainId
{
    public Guid Id { get; set; }
    
    [MaxLength(128, ErrorMessageResourceType = typeof(BASE.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    // [Display(Name = nameof(Name), Prompt = nameof(Name), ResourceType = typeof(APP.Resources.Domain.Wardrobe))]
    public string Name { get; set; } = default!;
    
    [MaxLength(500, ErrorMessageResourceType = typeof(BASE.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    // [Display(Name = nameof(Description), Prompt = nameof(Description), ResourceType = typeof(APP.Resources.Domain.Wardrobe))]
    public string? Description { get; set; }
    
    // [Display(Name = nameof(UserId), ResourceType = typeof(App.Resources.Domain.Wardrobe))]
    public Guid UserId { get; set; }
    
    // [Display(Name = nameof(User), ResourceType = typeof(App.Resources.Domain.Wardrobe))]
    public AppUser? User { get; set; }
    
    public ICollection<ClothingItem>? ClothingItems { get; set; }
}