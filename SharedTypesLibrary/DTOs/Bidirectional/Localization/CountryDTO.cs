using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedTypesLibrary.DTOs.Bidirectional.Localization;

public class CountryDTO
{
    public int Id { get; set; }

    public required string Name { get; set; } = string.Empty;

    public bool IsEnabled { get; set; }
}
