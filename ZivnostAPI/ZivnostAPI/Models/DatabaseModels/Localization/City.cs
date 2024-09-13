using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ZivnostAPI.Models.DatabaseModels.Localization;

[Table(nameof(City))]
public class City
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(District))]
    [Required]
    public required int District_Id { get; set; }

    [Required]
    public required string Name { get; set; } = string.Empty;
}