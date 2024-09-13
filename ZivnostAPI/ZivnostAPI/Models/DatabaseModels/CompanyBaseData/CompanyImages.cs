using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZivnostAPI.Models.DatabaseModels.CompanyBaseData;

[Table(nameof(CompanyImages))]
public class CompanyImages
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Company))]
    [Required]
    public required int Company_Id { get; set; }

    [Required]
    public string ImgPath { get; set; } = string.Empty;
}
