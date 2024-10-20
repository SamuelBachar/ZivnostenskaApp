using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SharedTypesLibrary.DTOs.Bidirectional.Localization;
public class CityDTO : BaseDTO
{
    public int Id { get; set; }

    public required int District_Id { get; set; }

    public required string Name { get; set; } = string.Empty;

    public override string DisplayName => Name;
}