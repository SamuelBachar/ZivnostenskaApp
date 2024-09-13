using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedTypesLibrary.DTOs.Bidirectional.Services;

public class Service
{
    public int Id { get; set; }

    public int Category_Id { get; set; }

    public required string Name { get; set; } = string.Empty;
}
