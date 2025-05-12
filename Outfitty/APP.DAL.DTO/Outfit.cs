using System.ComponentModel.DataAnnotations;
using BASE.Contracts;
using Domain;
using Domain.Enums;
using Domain.identity;

namespace APP.DAL.DTO;

public class Outfit : IDomainId
{
    public Guid Id { get; set; }
    
    [MaxLength(128, ErrorMessageResourceType = typeof(BASE.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    // [Display(Name = nameof(Name), Prompt = nameof(Name), ResourceType = typeof(APP.Resources.Domain.Outfit))]
    public string Name { get; set; } = default!;

    [MaxLength(500, ErrorMessageResourceType = typeof(BASE.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    // [Display(Name = nameof(Description), Prompt = nameof(Description), ResourceType = typeof(APP.Resources.Domain.Outfit))]
    public string? Description { get; set; }

    // [Display(Name = nameof(Season), Prompt = nameof(Season), ResourceType = typeof(APP.Resources.Domain.Outfit))]
    public ClothingSeason Season { get; set; }

    // [Display(Name = nameof(Style), Prompt = nameof(Style), ResourceType = typeof(APP.Resources.Domain.Outfit))]
    public ClothingStyle Style { get; set; }

    // [Display(Name = nameof(UserId), ResourceType = typeof(APP.Resources.Domain.Outfit))]
    public Guid UserId { get; set; }

    // [Display(Name = nameof(User), ResourceType = typeof(APP.Resources.Domain.Outfit))]
    public AppUser? User { get; set; }

    public ICollection<OutfitItem>? OutfitItems { get; set; }
    public ICollection<PlannerEntry>? PlannerEntries { get; set; }
    public ICollection<Favourite>? Favourites { get; set; }
}