using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZivnostAPI.Models.DatabaseModels.CompanyData;

[Table(nameof(CompanyLogo))]
public class CompanyLogo
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Company))]
    [Required]
    public required int Company_Id { get; set; }

    [Required]
    public string ImageName { get; set; } = string.Empty;
}
