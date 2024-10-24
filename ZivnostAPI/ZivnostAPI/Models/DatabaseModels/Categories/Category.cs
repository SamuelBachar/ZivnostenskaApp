using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZivnostAPI.Models.DatabaseModels.Services;

namespace ZivnostAPI.Models.DatabaseModels.Categories;

[Table(nameof(Category))]
public class Category
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Service))]
    public required int Service_Id { get; set; }

    public required string Name { get; set; } = string.Empty;
}