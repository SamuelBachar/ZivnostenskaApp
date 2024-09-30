using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedTypesLibrary.DTOs.Bidirectional.Localization;

public class RegionDTO
{
    public int Id { get; set; }

    public required int Country_Id { get; set; }

    public required string Name { get; set; } = string.Empty;
}
