using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedTypesLibrary.DTOs.Bidirectional.Localization;
using ZivnostAPI.Controllers.Generic;
using ZivnostAPI.Models.DatabaseModels.Localization;
using ZivnostAPI.Services.Interfaces;

namespace ZivnostAPI.Controllers;

[Route("api/CityController")]
[ApiController]
public class CityController : ReadController<City, CityDTO>
{
    private readonly IGenericReadOnlyService<City> _districtService;
    private readonly IMapper _mapper;
    public CityController(IGenericReadOnlyService<City> service, IMapper mapper) : base(service, mapper)
    {
        _districtService = service;
        _mapper = mapper;
    }
}