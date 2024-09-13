using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedTypesLibrary.DTOs.Bidirectional.Services;

public class CompanyService
{
    public int Id { get; set; }
    public required int Company_Id { get; set; }

    public required int Service_Id { get; set; }
}
