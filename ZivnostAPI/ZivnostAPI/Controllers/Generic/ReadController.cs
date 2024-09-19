using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZivnostAPI.Services.Generic;

namespace ZivnostAPI.Controllers.Generic;

[Route("api/[controller]")]
[ApiController]
public class ReadController<T> : ControllerBase where T : class
{
    private readonly IReadOnlyService<T> _readOnlyService;
    public ReadController(IReadOnlyService<T> service)
    {
        _readOnlyService = service;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<List<T>>> GetAll()
    {
        var entities = await _readOnlyService.GetAll();
        return Ok(entities);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<T>> GetById(int id)
    {
        var entity = await _readOnlyService.GetById(id);

        if (entity == null)
            return NotFound($"{typeof(T).Name} not found");

        return Ok(entity);
    }
}
