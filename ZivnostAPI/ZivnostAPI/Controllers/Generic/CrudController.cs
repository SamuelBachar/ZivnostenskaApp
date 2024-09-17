using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZivnostAPI.Services.Generic;

namespace ZivnostAPI.Controllers.Generic;

[Route("api/[controller]")]
[ApiController]
public class CrudController<T> : ControllerBase where T : class
{
    private readonly ICrudService<T> _crudService;
    public CrudController(ICrudService<T> service)
    {
        _crudService = service;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<List<T>>> GetAll()
    {
        var entities = await _crudService.GetAll();
        return Ok(entities);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<T>> GetById(int id)
    {
        var entity = await _crudService.GetById(id);
        if (entity == null) return NotFound($"{typeof(T).Name} not found");
        return Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<T>> Add([FromBody] T entity)
    {
        ActionResult<T> result;
        bool uResult = await _crudService.Add(entity);

        if (uResult)
        {
            result = BadRequest(uResult);
        }
        else
        {
            result = Ok(uResult);
        }

        return result;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<T>> Update(int id, [FromBody] T entity)
    {
        ActionResult<T> result;
        bool uResult = await _crudService.Update(id, entity);

        if (uResult)
        {
            result = BadRequest(uResult);
        }
        else
        {
            result = Ok(uResult);
        }

        return result;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<List<T>>> Delete(int id)
    {
        ActionResult<T> result;
        bool uResult = await _crudService.Delete(id);

        if (uResult)
        {
            result = BadRequest(uResult);
        }
        else
        {
            result = Ok(uResult);
        }

        return Ok(result);
    }

}
