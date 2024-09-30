using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedTypesLibrary.DTOs.Bidirectional.Localization;
using SharedTypesLibrary.ServiceResponseModel;
using ZivnostAPI.Controllers.Generic;
using ZivnostAPI.Services.Interfaces;
using Region = ZivnostAPI.Models.DatabaseModels.Localization.Region;


namespace ZivnostAPI.Controllers;


[Route("api/RegionController")]
[ApiController]
public class RegionController : ReadController<Region, RegionDTO>
{
    private readonly IGenericReadOnlyService<Region> _readOnlyService;
    private readonly IMapper _mapper;

    public RegionController(IGenericReadOnlyService<Region> service, IMapper mapper) : base(service, mapper)
    {
        _readOnlyService = service;
        _mapper = mapper;
    }
}
