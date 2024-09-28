using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZivnostAPI.Controllers.Generic;
using ZivnostAPI.Services.Generic;
using Region = ZivnostAPI.Models.DatabaseModels.Localization.Region;

namespace ZivnostAPI.Controllers;


[Route("api/RegionController")]
[ApiController]
public class RegionController : ReadController<Region>
{
    private readonly IReadOnlyService<Region> _readOnlyService;
    public RegionController(IReadOnlyService<Region> service) : base(service)
    {
        _readOnlyService = service;
    }

    //[HttpGet("GetAll")]
    //public override async Task<ActionResult<List<Region>>> GetAll()
    //{
    //    return await base.GetAll(); // Calls the method in ReadController
    //}

    //[HttpGet("{id}")]
    //public override async Task<ActionResult<Region>> GetById(int id)
    //{
    //    return await base.GetById(id); // Calls the method in ReadController
    //}
}
