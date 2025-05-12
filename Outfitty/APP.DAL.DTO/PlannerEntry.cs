using System.ComponentModel.DataAnnotations;
using BASE.Contracts;
using Domain;
using Domain.identity;

namespace APP.DAL.DTO;

public class PlannerEntry : IDomainId
{
    public Guid Id { get; set; }
    
    // [Display(Name = nameof(Date), Prompt = nameof(Date), ResourceType = typeof(App.Resources.Domain.PlannerEntry))]
    public DateTime Date { get; set; }

    [MaxLength(128, ErrorMessageResourceType = typeof(BASE.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    // [Display(Name = nameof(Title), Prompt = nameof(Title), ResourceType = typeof(APP.Resources.Domain.PlannerEntry))]
    public string Title { get; set; } = default!;

    [MaxLength(500, ErrorMessageResourceType = typeof(BASE.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    // [Display(Name = nameof(Comment), Prompt = nameof(Comment), ResourceType = typeof(APP.Resources.Domain.PlannerEntry))]
    public string? Comment { get; set; }

    // [Display(Name = nameof(Time), Prompt = nameof(Time), ResourceType = typeof(APP.Resources.Domain.PlannerEntry))]
    public TimeSpan? Time { get; set; }

    // [Display(Name = nameof(UserId), ResourceType = typeof(APP.Resources.Domain.PlannerEntry))]
    public Guid UserId { get; set; }

    // [Display(Name = nameof(User), ResourceType = typeof(APP.Resources.Domain.PlannerEntry))]
    public AppUser? User { get; set; }

    // [Display(Name = nameof(OutfitId), ResourceType = typeof(APP.Resources.Domain.PlannerEntry))]
    public Guid OutfitId { get; set; }

    // [Display(Name = nameof(Outfit), ResourceType = typeof(APP.Resources.Domain.PlannerEntry))]
    public Outfit? Outfit { get; set; }
}