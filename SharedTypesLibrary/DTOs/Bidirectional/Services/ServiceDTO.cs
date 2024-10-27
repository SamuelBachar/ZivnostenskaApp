using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedTypesLibrary.DTOs.Bidirectional.Services;

public class ServiceDTO
{
    public int Id { get; set; }

    public required string Name { get; set; } = string.Empty;

    public string ImageURL {  get; set; } = string.Empty;
}
