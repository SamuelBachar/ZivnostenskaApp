using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SharedTypesLibrary.DTOs.Bidirectional.Localization;
public class City
{
    public int Id { get; set; }

    public required int District_Id { get; set; }

    public required string Name { get; set; } = string.Empty;
}