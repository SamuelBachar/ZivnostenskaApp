using SharedTypesLibrary.DTOs.Bidirectional.Localization;
using SharedTypesLibrary.DTOs.Bidirectional.Services;

namespace SharedTypesLibrary.DTOs.Request;

public class RegistrationCompanyDataRequest
{
    public string CompanyName { get; set; } = string.Empty;
    public string CIN { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public Country Country { get; set; }
    public City City { get; set; }
    public Region RegionCompany { get; set; }
    public District DistrictCompany { get; set; }
    public byte[] Image { get; set; }

    public string CompanyDescription { get; set; } = string.Empty;

    public List<Service> ListServices { get; set; }
}
