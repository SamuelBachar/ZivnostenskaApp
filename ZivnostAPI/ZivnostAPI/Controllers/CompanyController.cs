using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using ZivnostAPI.Models;
using ZivnostAPI.Models.DatabaseModels.Company;
using ZivnostAPI.Services.CompanyService;

namespace ZivnostAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet("GetAllCompanies")]
        public async Task<ActionResult<List<Company>>> GetAllCompanies()
        {
            List<Company> listCompanies = await _companyService.GetAllCompanies();
            return Ok(listCompanies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetSpecificCompany(int id)
        {
            ObjectResult? result = null;
            Company? company = await _companyService.GetSpecificCompany(id);

            if (company == null)
                result = NotFound("Specific company not found");
            else
                result = Ok(company);

            return result;
        }

        [HttpPost]
        public async Task<ActionResult<List<Company>>> AddCompany([FromBody]Company reqCompany)
        {
            List<Company> listCompanies = await _companyService.AddCompany(reqCompany);
            return Ok(listCompanies);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Company>> UpdateSpecificCompany(int id, Company reqCompany)
        {
            ObjectResult? result = null;
            Company? company = await _companyService.UpdateSpecificCompany(id, reqCompany);

            if (company == null)
            {
                result = NotFound("Company not found");
            }
            else
            {
                result = Ok(company);
            }

            return result;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Company>>> DeleteSpecificCompany(int id)
        {
            ObjectResult? result = null;
            List<Company>? listCompany = await _companyService.DeleteSpecificCompany(id);

            if (listCompany == null)
            {
                result = NotFound("Company not found");
            }
            else
            {
                result = Ok(listCompany);
            }

            return result;
        }
    }
}
