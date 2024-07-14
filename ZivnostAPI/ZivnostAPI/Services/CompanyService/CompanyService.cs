using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZivnostAPI.Data.DataContext;
using ZivnostAPI.Models.CompanyBaseData;

namespace ZivnostAPI.Services.CompanyService;

public class CompanyService : ICompanyService
{
    private readonly DataContext _dataContext;

    public CompanyService(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpGet]
    public async Task<List<Company>> GetAllCompanies()
    {
        return await _dataContext.Company.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<Company?> GetSpecificCompany(int id)
    {
        Company? company = await _dataContext.Company.FindAsync(id);
        return company;
    }

    [HttpPost]
    public async Task<List<Company>> AddCompany([FromBody] Company company)
    {
        await _dataContext.Company.AddAsync(company);
        await _dataContext.SaveChangesAsync();
        return await _dataContext.Company.ToListAsync();
    }

    [HttpPut("{id}")]
    public async Task<Company?> UpdateSpecificCompany([FromBody] int id, [FromBody] Company reqCompany)
    {
        Company? company = await _dataContext.Company.FindAsync(id);

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

        await _dataContext.SaveChangesAsync();

        return company;
    }

    [HttpDelete("{id}")]
    public async Task<List<Company>?> DeleteSpecificCompany(int id)
    {
        List<Company>? result = null;
        Company? company = await _dataContext.Company.FindAsync(id);

        if (company == null)
            result = null;
        else
        {
            _dataContext.Company.Remove(company);
            await _dataContext.SaveChangesAsync();

            result = await _dataContext.Company.ToListAsync();
        }

        return result;
    }
}
