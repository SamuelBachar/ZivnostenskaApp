using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZivnostAPI.Models.CompanyBaseData;

namespace ZivnostAPI.Models.Localization;

[Table(nameof(Country))]
public class Country
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; } = string.Empty;
}
