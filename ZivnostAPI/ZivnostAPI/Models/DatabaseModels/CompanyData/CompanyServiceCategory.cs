using SharedTypesLibrary.DTOs.Bidirectional.Categories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZivnostAPI.Models.DatabaseModels.Services;

namespace ZivnostAPI.Models.DatabaseModels.CompanyData;

[Table(nameof(CompanyServiceCategory))]
public class CompanyServiceCategory
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Company))]
    public required int Company_Id { get; set; }

    [ForeignKey(nameof(Service))]
    public required int Service_Id { get; set; }

    [ForeignKey(nameof(CategoryDTO))]
    public required int Category_Id { get; set; }
}
