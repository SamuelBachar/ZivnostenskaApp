using SharedTypesLibrary.DTOs.Bidirectional.Localization;
using SharedTypesLibrary.DTOs.Bidirectional.Services;

namespace SharedTypesLibrary.DTOs.Request;

public class RegistrationCompanyRequest
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string CIN { get; set; } = string.Empty;

    public required string Phone { get; set; } = string.Empty;

    public required string Email { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public required CountryDTO Country { get; set; }
    public required CityDTO City { get; set; }
    public required RegionDTO RegionCompany { get; set; }
    public required DistrictDTO DistrictCompany { get; set; }
    public byte[] Image { get; set; }

    public string CompanyDescription { get; set; } = string.Empty;

    public required List<ServiceDTO> ListServices { get; set; }

    // Register via Auth Provider
    RegisterAuthProviderCredentials? RegProviderData { get; set; }

    // Register via Generic method (email and confirmation)

    RegisterGenericCredentials? RegGenericData { get; set; }
}
