using Microsoft.AspNetCore.Mvc;
using ZivnostAPI.Models.CompanyBaseData;

namespace ZivnostAPI.Services.CompanyService;

public class CompanyService : ICompanyService
{
    private static List<Company> listCompanies = new List<Company>
    {
        new Company{ Id = 1, Name = "VABA Solution s.r.o.", City_Id = 1},
        new Company{ Id = 2, Name = "Fuseri s.r.o.", City_Id = 1}
    };

    [HttpGet]
    public List<Company> GetAllCompanies()
    {
        return listCompanies;
    }

    [HttpGet("{id}")]
    public Company? GetSpecificCompany(int id)
    {
        Company? company = listCompanies.Find(company => company.Id == id);
        return company;
    }

    [HttpPost]
    public List<Company> AddCompany([FromBody] Company company)
    {
        listCompanies.Add(company);
        return listCompanies;
    }

    [HttpPut("{id}")]
    public Company? UpdateSpecificCompany([FromBody] int id, [FromBody] Company reqCompany)
    {
        Company? company = listCompanies.Find(company => company.Id == id);

        if (company != null)
        {
            company.Name = reqCompany.Name;
            company.ICO = reqCompany.ICO;
            company.DIC = reqCompany.DIC;
            company.Description = reqCompany.Description;

            company.Email = reqCompany.Email;
            company.PhoneNumber = reqCompany.PhoneNumber;

            company.City_Id = reqCompany.City_Id;
            company.Street = reqCompany.Street;
            company.PostalCode = reqCompany.PostalCode;

            company.Subscribed = reqCompany.Subscribed;
            company.SubscriptionFrom = reqCompany.SubscriptionFrom;
            company.SubscriptionTo = reqCompany.SubscriptionTo;
            company.RegisteredAt = reqCompany.RegisteredAt;
        }

        return company;
    }

    [HttpDelete("{id}")]
    public List<Company>? DeleteSpecificCompany(int id)
    {
        List<Company>? result = null;
        Company? company = listCompanies.Find(company => company.Id == id);

        if (company == null)
            result = null;
        else
        {
            listCompanies.Remove(company);
            result = listCompanies;
        }

        return result;
    }
}
