using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZivnostAPI.Models.DatabaseModels.Company;

namespace ZivnostAPI.Services.CompanyService;

public interface ICompanyService
{
    [HttpGet]
    Task<List<Company>> GetAllCompanies();

    [HttpGet("{id}")]
    Task<Company?> GetSpecificCompany(int id);

    [HttpPost]
    Task<List<Company>> AddCompany([FromBody] Company company);

    [HttpPut("{id}")]
    Task<Company?> UpdateSpecificCompany([FromBody] int id, [FromBody] Company reqCompany);

    [HttpDelete("{id}")]
    Task<List<Company>?> DeleteSpecificCompany(int id);
}
