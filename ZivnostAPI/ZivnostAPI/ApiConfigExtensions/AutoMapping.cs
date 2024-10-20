using AutoMapper;
using SharedTypesLibrary.DTOs.Bidirectional.Localization;
using ZivnostAPI.Models.DatabaseModels.Localization;

namespace ZivnostAPI.ApiConfigExtensions;

public class AutoMappingProfile : Profile
{
    public AutoMappingProfile()
    {
        CreateMap<Region, RegionDTO>();
        CreateMap<District, DistrictDTO>();
        CreateMap<City, CityDTO>();
    }
}

public static class AutoMapping
{
    public static IServiceCollection AddAutoMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AutoMappingProfile));

        return services;
    }
}
