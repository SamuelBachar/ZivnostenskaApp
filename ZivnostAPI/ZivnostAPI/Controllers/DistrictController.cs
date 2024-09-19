using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedTypesLibrary.DTOs.Bidirectional.Localization;
using ZivnostAPI.Controllers.Generic;
using ZivnostAPI.Services.Generic;

namespace ZivnostAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DistrictController : ReadController<District>
{
    IReadOnlyService<District> _districtService;
    public DistrictController(IReadOnlyService<District> service) : base(service)
    {
        _districtService = service;
    }
}
