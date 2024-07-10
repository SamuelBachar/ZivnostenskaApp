using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZivnostAPI.Models.CompanyBaseData;

namespace ZivnostAPI.Services.CompanyService;

public interface ICompanyService
{
    [HttpGet]
    List<Company> GetAllCompanies();

    [HttpGet("{id}")]
    Company? GetSpecificCompany(int id);

    [HttpPost]
    List<Company> AddCompany([FromBody] Company company);

    [HttpPut("{id}")]
    Company? UpdateSpecificCompany([FromBody] int id, [FromBody] Company reqCompany);

    [HttpDelete("{id}")]
    List<Company>? DeleteSpecificCompany(int id);
}
