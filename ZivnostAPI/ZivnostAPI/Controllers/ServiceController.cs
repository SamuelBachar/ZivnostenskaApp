using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SharedTypesLibrary.DTOs.Bidirectional.Localization;
using SharedTypesLibrary.DTOs.Bidirectional.Services;
using ZivnostAPI.Controllers.Generic;
using ZivnostAPI.Models.DatabaseModels.Services;
using ZivnostAPI.Services.Interfaces;

namespace ZivnostAPI.Controllers;

[Route("api/ServiceController")]
[ApiController]
public class ServiceController : ReadController<Service, ServiceDTO>
{
    private readonly IGenericReadOnlyService<Service> _readOnlyService;
    private readonly IMapper _mapper;

    public ServiceController(IGenericReadOnlyService<Service> service, IMapper mapper) : base(service, mapper)
    {
        _readOnlyService = service;
        _mapper = mapper;
    }
}
