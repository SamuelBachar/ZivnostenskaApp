using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZivnostAPI.Models.DatabaseModels.Localization;

[Table(nameof(Country))]
public class Country
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; } = string.Empty;

    public bool IsEnabled { get; set; }
}
