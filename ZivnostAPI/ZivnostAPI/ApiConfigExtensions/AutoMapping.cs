using AutoMapper;
using Microsoft.Extensions.Configuration;
using SharedTypesLibrary.DTOs.Bidirectional.Categories;
using SharedTypesLibrary.DTOs.Bidirectional.Localization;
using SharedTypesLibrary.DTOs.Bidirectional.Services;
using ZivnostAPI.Models.DatabaseModels.Categories;
using ZivnostAPI.Models.DatabaseModels.Localization;
using ZivnostAPI.Models.DatabaseModels.Services;
using ZivnostAPI.Models.JWT;

namespace ZivnostAPI.ApiConfigExtensions;

public class AutoMappingProfile : Profile
{
    private readonly string _apiStaticFilesUrl;

    public AutoMappingProfile(string apiStaticFilesUrl)
    {
        _apiStaticFilesUrl = apiStaticFilesUrl;

        CreateMap<Country, CountryDTO>();
        CreateMap<Region, RegionDTO>();
        CreateMap<District, DistrictDTO>();
        CreateMap<City, CityDTO>();
        CreateMap<Category, CategoryDTO>();

        CreateMap<Service, ServiceDTO>()
                .ForMember(dest => dest.ImageURL,
                opt => opt.MapFrom(src => $"{_apiStaticFilesUrl}/Images/Services/{src.ImageName}"));
    }
}

public static class AutoMapping
{
    public static IServiceCollection AddAutoMapping(this IServiceCollection services, IConfiguration configuration)
    {
        string? apiStaticFilesUrl = configuration.GetSection("ApiStaticFilesUrl")["Url"];

        if (apiStaticFilesUrl == null)
            throw new InvalidOperationException("Static files URL not found");

        services.AddAutoMapper(cfg => cfg.AddProfile(new AutoMappingProfile(apiStaticFilesUrl)));

        return services;
    }
}
