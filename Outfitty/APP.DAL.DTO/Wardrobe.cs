using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.Arm;
using BASE.Contracts;

namespace APP.DAL.DTO;

public class Wardrobe : IDomainId
{
    public Guid Id { get; set; }
    [MaxLength(128, ErrorMessageResourceType = typeof(BASE.Resources.Common))]
    public string Name { get; set; } = default!;
}