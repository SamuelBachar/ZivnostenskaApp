using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using ZivnostAPI.Models.CompanyBaseData;

namespace ZivnostAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private static List<Company> listCompanies = new List<Company>
        {
            new Company{ Id = 1, Name = "VABA Solution s.r.o."},
            new Company{ Id = 2, Name = "Fuseri s.r.o."}
        };

        [HttpGet]
        public async Task<ActionResult<List<Company>>> GetAllCompanies()
        {
            return Ok(listCompanies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Company>>> GetSpecificCompany(int id)
        {
            Company? company = listCompanies.Find(company => company.Id == id);
            return Ok(company);
        }
    }
}
