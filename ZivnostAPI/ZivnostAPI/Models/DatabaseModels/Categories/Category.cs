using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZivnostAPI.Models.DatabaseModels.Categories;

[Table(nameof(Category))]
public class Category
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Category))]
    public int? Parrent_Category_Id { get; set; }

    public required string Name { get; set; } = string.Empty;
}