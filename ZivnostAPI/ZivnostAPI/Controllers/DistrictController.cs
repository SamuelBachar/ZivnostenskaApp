using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZivnostAPI.Models.DatabaseModels.Localization;
using ZivnostAPI.Controllers.Generic;
using ZivnostAPI.Services.Interfaces;
using AutoMapper;
using SharedTypesLibrary.DTOs.Bidirectional.Localization;

namespace ZivnostAPI.Controllers;

[Route("api/DistrictController")]
[ApiController]
public class DistrictController : ReadController<District, DistrictDTO>
{
    private readonly IGenericReadOnlyService<District> _districtService;
    private readonly IMapper _mapper;
    public DistrictController(IGenericReadOnlyService<District> service, IMapper mapper) : base(service, mapper)
    {
        _districtService = service;
        _mapper = mapper;
    }
}
