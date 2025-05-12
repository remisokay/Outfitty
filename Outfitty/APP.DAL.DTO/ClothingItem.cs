using System.ComponentModel.DataAnnotations;
using BASE.Contracts;
using Domain.Enums;

namespace APP.DAL.DTO;

public class ClothingItem : IDomainId
{
    public Guid Id { get; set; }
    [MaxLength(128, ErrorMessageResourceType = typeof(BASE.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    // [Display(Name = nameof(Name), Prompt = nameof(Name), ResourceType = typeof(APP.Resources.Domain.ClothingItem))]
    public string Name { get; set; } = default!;

    // [Display(Name = nameof(Type), Prompt = nameof(Type), ResourceType = typeof(APP.Resources.Domain.ClothingItem))]
    public ClothingType Type { get; set; }

    // [Display(Name = nameof(Style), Prompt = nameof(Style), ResourceType = typeof(APP.Resources.Domain.ClothingItem))]
    public ClothingStyle Style { get; set; }

    // [Display(Name = nameof(Season), Prompt = nameof(Season), ResourceType = typeof(APP.Resources.Domain.ClothingItem))]
    public ClothingSeason Season { get; set; }

    // [Display(Name = nameof(PrimaryColor), Prompt = nameof(PrimaryColor), ResourceType = typeof(APP.Resources.Domain.ClothingItem))]
    public ClothingColor PrimaryColor { get; set; }

    // [Display(Name = nameof(SecondaryColor), Prompt = nameof(SecondaryColor), ResourceType = typeof(APP.Resources.Domain.ClothingItem))]
    public ClothingColor? SecondaryColor { get; set; }

    // [Display(Name = nameof(WardrobeId), ResourceType = typeof(APP.Resources.Domain.ClothingItem))]
    public Guid WardrobeId { get; set; }

    // [Display(Name = nameof(Wardrobe), ResourceType = typeof(APP.Resources.Domain.ClothingItem))]
    public Wardrobe? Wardrobe { get; set; }

    // [Display(Name = nameof(ImageMetadataId), ResourceType = typeof(APP.Resources.Domain.ClothingItem))]
    public Guid ImageMetadataId { get; set; }

    // [Display(Name = nameof(ImageMetadata), ResourceType = typeof(APP.Resources.Domain.ClothingItem))]
    public ImageMetadata? ImageMetadata { get; set; }
}