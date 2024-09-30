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

    public CountryDTO Country { get; set; }
    public CityDTO City { get; set; }
    public RegionDTO RegionCompany { get; set; }
    public DistrictDTO DistrictCompany { get; set; }
    public byte[] Image { get; set; }

    public string CompanyDescription { get; set; } = string.Empty;

    public List<Service> ListServices { get; set; }
}
