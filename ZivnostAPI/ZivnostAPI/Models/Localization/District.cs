using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZivnostAPI.Models.Localization;

[Table(nameof(District))]
public class District
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Region))]
    [Required]
    public required int Region_Id { get; set; }

    [Required]
    public required string Name { get; set; } = string.Empty;
}
