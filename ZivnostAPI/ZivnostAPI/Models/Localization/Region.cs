using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZivnostAPI.Models.CompanyBaseData;

namespace ZivnostAPI.Models.Localization;

[Table(nameof(Region))]
public class Region
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Country))]
    public required int Country_Id { get; set; }

    [Required]
    public required string Name { get; set; } = string.Empty;
}
