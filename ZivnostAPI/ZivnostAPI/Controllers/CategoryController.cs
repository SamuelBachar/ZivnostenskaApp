using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedTypesLibrary.DTOs.Bidirectional.Categories;
using ZivnostAPI.Controllers.Generic;
using ZivnostAPI.Models.DatabaseModels.Categories;
using ZivnostAPI.Services.Interfaces;

namespace ZivnostAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ReadController<Category, CategoryDTO>
{
    private readonly IGenericReadOnlyService<Category> _readOnlyService;
    private readonly IMapper _mapper;
    public CategoryController(IGenericReadOnlyService<Category> service, IMapper mapper) : base(service, mapper)
    {
        _readOnlyService = service;
        _mapper = mapper;
    }
}
