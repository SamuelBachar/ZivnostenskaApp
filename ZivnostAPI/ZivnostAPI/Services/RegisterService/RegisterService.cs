using Microsoft.EntityFrameworkCore;
using SharedTypesLibrary.DbResponse;
using SharedTypesLibrary.DTOs.Request;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.ServiceResponseModel;
using ZivnostAPI.Data.CusDbContext;
using ZivnostAPI.Models.DatabaseModels.Account;
using ExtensionsLibrary.DbExtensions;
using ZivnostAPI.Models.DatabaseModels.Company;
using System.Numerics;
using System.Net;
using ZivnostAPI.Models.DatabaseModels.CompanyData;
using Azure.Core;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace ZivnostAPI.Services.RegisterService;

public class RegisterService : IRegisterService
{
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly CusDbContext _dbContext;

    public RegisterService(IHttpClientFactory httpClientFactory, CusDbContext dataContext)
    {
        _dbContext = dataContext;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ApiResponse<RegistrationCompanyResponse>> RegisterCompany(RegistrationCompanyRequest request, IFormFile image)
    {
        ApiResponse<RegistrationCompanyResponse> response = new ApiResponse<RegistrationCompanyResponse>();

        try
        {
            if (request == null)
            {
                response.Success = false;
            }
            else
            {
                Company company = await CreateNewCompany(request);

                if (request.IsRegisteredByOAuth)
                {
                    Account accountOAuth = await UpdateOAuthAccountData(request, company);

                    // The user has also provided alternative registration data to enable login/registration through the Generic Method
                    if (request.RegGenericData != null)
                    {
                        SetGenericRegData(request, accountOAuth);
                    }
                }
                else
                {
                    Account? accountGeneric = await GetAccountIfAlreadyExists(request);

                    if (accountGeneric != null)
                    {
                        if (accountGeneric.VerifiedAt == null)
                        {
                            response.Success = false;
                            response.ApiErrorCode = "UAE_751";
                        }
                    }
                    else
                    {
                        Account accRegAccGeneric = await CreateNewAccount(request, company);
                        SetGenericRegData(request, accRegAccGeneric);
                    }
                }

                if (image != null && image.Length > 0)
                {
                    await SaveCompanyLogo(company, image);
                }

                DbActionResponse dbResult = await _dbContext.ExtSaveChangesAsync();

                if (dbResult.IsSucces)
                {
                    response.Success = true;
                    // todo create JWT
                }
                else
                {
                    response.Success = false;
                    response.ApiErrorCode = dbResult.ApiErrorCode;
                    response.APIException = dbResult.Exception;
                }
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ApiErrorCode = "UAE_750";
            response.APIException = ex.Message;
        }

        return response;
    }

    private string GetImageFormat(string imageContentType)
    {
        string resultType = string.Empty;

        Dictionary<string, string> dicTypes = new Dictionary<string, string>
        {
            { "image/jpeg", ".jpg" },
            { "image/png", ".png" }
        };

        foreach (var type in dicTypes)
        {
            if (imageContentType.Contains(type.Key))
            {
                resultType = type.Value;
                break;
            }
        }

        return resultType;
    }

    private async Task<Company> CreateNewCompany(RegistrationCompanyRequest request)
    {
        Company company = new Company
        {
            Name = request.CompanyName,
            CIN = request.CIN,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            PostalCode = request.PostalCode,
            Country_Id = request.Country.Id,
            Region_Id = request.Region.Id,
            District_Id = request.District.Id,
            Description = request.Description,
            RegisteredAt = DateTime.Now,
        };

        await _dbContext.Company.AddAsync(company);

        return company;
    }

    private async Task<Account> UpdateOAuthAccountData(RegistrationCompanyRequest request, Company company)
    {
        Account account = await _dbContext.Account.FindAsync(request.Id);
        account.IsCompanyAccount = true;
        account.RegisteredAsCompanyAt = DateTime.Now;
        account.Company_Id = company.Id;

        return account;
    }

    private async Task SaveCompanyLogo(Company company, IFormFile image)
    {
        string imgFormat = GetImageFormat(image.ContentType);
        string imgName = Guid.NewGuid().ToString() + imgFormat;
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\Images\\Company\\Logo\\{imgName}");

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }

        CompanyLogo compImg = new CompanyLogo
        {
            Company_Id = company.Id,
            ImageName = imgName
        };

        await _dbContext.CompanyLogo.AddAsync(compImg);
    }

    private string CreatePasswordHashWithSalt(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private string CreateRandomToken()
    {
        string result = string.Empty;

        do
        {
            result = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        } while (_dbContext.Account.Any(u => u.VerificationToken == result));

        return result;
    }

    private void SetGenericRegData(RegistrationCompanyRequest request, Account account)
    {
        account.Email = request.RegGenericData.Email;
        account.PasswordHashWithSalt = CreatePasswordHashWithSalt(request.RegGenericData.Password);
        account.VerificationToken = CreateRandomToken();
    }

    // User might be already registered as Customer
    private async Task<Account?> GetAccountIfAlreadyExists(RegistrationCompanyRequest request)
    {
        Account? account = null;

        if (request.Id != -1)
        {
            account = await _dbContext.Account.FindAsync(request.Id);
        }
        else
        {
            account = await _dbContext.Account.FirstOrDefaultAsync(account => account.Email == request.Email);
        }

        return account;
    }

    private async Task<Account> CreateNewAccount(RegistrationCompanyRequest request, Company company)
    {
        Account account = new Account
        {
            Email = request.RegGenericData.Email,
            IsCompanyAccount = true,
            IsCustomerAccount = false,
            Company_Id = company.Id,
        };

        return account;
    }
}
