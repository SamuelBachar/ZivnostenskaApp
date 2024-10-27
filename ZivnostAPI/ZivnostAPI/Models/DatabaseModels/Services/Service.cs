using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZivnostAPI.Models.DatabaseModels.Services;

[Table(nameof(Service))]
public class Service
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; } = string.Empty;

    public string ImageName { get; set; } = string.Empty;
}
