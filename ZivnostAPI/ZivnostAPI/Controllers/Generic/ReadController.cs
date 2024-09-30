using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedTypesLibrary.ServiceResponseModel;
using ZivnostAPI.Services.Interfaces;

namespace ZivnostAPI.Controllers.Generic;

[Route("api/[controller]")]
[ApiController]
public class ReadController<TEntity, TDto> : ControllerBase where TEntity : class
{
    private readonly IGenericReadOnlyService<TEntity> _readOnlyService;
    private readonly IMapper _mapper;
    public ReadController(IGenericReadOnlyService<TEntity> service, IMapper mapper)
    {
        _readOnlyService = service;
        _mapper = mapper;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<ApiResponse<TDto>>> GetAll()
    {
        ActionResult response;

        var result = await _readOnlyService.GetAll();

        if (result.Success)
        {
            var dtoList = _mapper.Map<IEnumerable<TDto>>(result.ListData);
            response = Ok(new ApiResponse<IEnumerable<TDto>>(dtoList, true));
        }
        else
        {
            response = BadRequest(new ApiResponse<TDto>(listData: null, false, result.Message, result.ExceptionMessage));
        }

        return response;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TDto>>> GetById(int id)
    {
        ActionResult response;

        var result = await _readOnlyService.GetById(id);

        if (result.Success)
        {
            var dto = _mapper.Map<TDto>(result.Data);
            response = Ok(new ApiResponse<TDto>(dto, true));
        }
        else
        {
            response = NotFound(new ApiResponse<TDto>(data: default(TDto), false, result.Message, result.ExceptionMessage));
        }

        return response;
    }
}
