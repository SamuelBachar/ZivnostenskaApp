using SharedTypesLibrary.DTOs.Bidirectional.Categories;
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
    public required RegionDTO Region { get; set; }
    public required DistrictDTO District { get; set; }

    public string Description { get; set; } = string.Empty;

    public required List<CategoryDTO> ListCategories { get; set; }

    // Register via Generic method (email and confirmation)
    public RegisterGenericCredentials? RegGenericData { get; set; }

    public bool IsRegisteredByOAuth { get; set; }
    public bool IsRegisteredByGenericMethod { get; set; }
}
