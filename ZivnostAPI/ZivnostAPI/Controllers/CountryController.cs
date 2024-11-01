using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedTypesLibrary.DTOs.Bidirectional.Localization;
using ZivnostAPI.Controllers.Generic;
using ZivnostAPI.Models.DatabaseModels.Localization;
using ZivnostAPI.Services.Interfaces;

namespace ZivnostAPI.Controllers;

[Route("api/CountryController")]
[ApiController]
public class CountryController : ReadController<Country, CountryDTO>
{
    private readonly IGenericReadOnlyService<Country> _countryService;
    private readonly IMapper _mapper;
    public CountryController(IGenericReadOnlyService<Country> service, IMapper mapper) : base(service, mapper)
    {
        _countryService = service;
        _mapper = mapper;
    }
}