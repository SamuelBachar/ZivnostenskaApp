using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZivnostAPI.Models.DatabaseModels.Localization;
using ZivnostAPI.Controllers.Generic;
using ZivnostAPI.Services.Generic;

namespace ZivnostAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DistrictController : ReadController<District>
{
    private readonly IReadOnlyService<District> _districtService;
    public DistrictController(IReadOnlyService<District> service) : base(service)
    {
        _districtService = service;
    }
}
