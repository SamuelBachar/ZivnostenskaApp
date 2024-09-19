using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZivnostAPI.Controllers.Generic;
using ZivnostAPI.Services.Generic;
using Region = ZivnostAPI.Models.DatabaseModels.Localization.Region;

namespace ZivnostAPI.Controllers;



[Route("api/[controller]")]
[ApiController]
public class RegionContoller : ReadController<Region>
{
    private readonly IReadOnlyService<Region> _readOnlyService;
    public RegionContoller(IReadOnlyService<Region> service) : base(service)
    {
        _readOnlyService = service;
    }
}
