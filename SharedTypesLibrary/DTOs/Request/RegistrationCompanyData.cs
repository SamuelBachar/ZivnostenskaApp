using SharedTypesLibrary.DTOs.Bidirectional.Localization;
using SharedTypesLibrary.DTOs.Bidirectional.Services;

namespace SharedTypesLibrary.DTOs.Request;

public class RegistrationCompanyData
{
    string CompanyName { get; set; } = string.Empty;
    string CIN { get; set; } = string.Empty;

    string Phone { get; set; } = string.Empty;

    string Email { get; set; } = string.Empty;

    string Address { get; set; } = string.Empty;

    string PostalCode { get; set; } = string.Empty;

    public City City { get; set; }
    public Region Region { get; set; }
    public District District { get; set; }
    public byte[] Image { get; set; }

    string CompanyDesctiption { get; set; } = string.Empty;

    List<Service> ListServices { get; set; }
}
