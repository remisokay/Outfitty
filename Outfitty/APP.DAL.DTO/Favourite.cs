using System.ComponentModel.DataAnnotations;
using BASE.Contracts;
using Domain.identity;

namespace APP.DAL.DTO;

public class Favourite : IDomainId
{
    public Guid Id { get; set; }
    // [Display(Name = nameof(UserId), ResourceType = typeof(APP.Resources.Domain.Favourite))]
    public Guid UserId { get; set; }

    // [Display(Name = nameof(User), ResourceType = typeof(APP.Resources.Domain.Favourite))]
    public AppUser? User { get; set; }

    // [Display(Name = nameof(OutfitId), ResourceType = typeof(APP.Resources.Domain.Favourite))]
    public Guid OutfitId { get; set; }

    // [Display(Name = nameof(Outfit), ResourceType = typeof(APP.Resources.Domain.Favourite))]
    public Outfit? Outfit { get; set; }
}