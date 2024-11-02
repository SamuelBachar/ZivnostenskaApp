using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedTypesLibrary.ServiceResponseModel;
using ZivnostAPI.Data.CusDbContext;
using ZivnostAPI.Models.DatabaseModels.Account;
using static SharedTypesLibrary.Enums.Enums;
using ExtensionsLibrary.DbExtensions;
using SharedTypesLibrary.DbResponse;
using SharedTypesLibrary.DTOs.Bidirectional.Account;

namespace ZivnostAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly CusDbContext _dbContext;

        public AccountController(CusDbContext dbContext)
        {
            _dbContext = dbContext;    
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<UpdateAccountTypeDTO>>> UpdateAccountType(UpdateAccountTypeDTO data)
        {
            ObjectResult result;
            ApiResponse<UpdateAccountTypeDTO> response = new ();

            try
            {
                Account account = await _dbContext.Set<Account>().FindAsync(data.Id);

                if (account != null)
                {
                    if (data.AccountType == AccountType.Company)
                    {
                        account.IsCompanyAccount = true;
                        account.RegisteredAsCompanyAt = DateTime.UtcNow;
                    }

                    if (data.AccountType == AccountType.Customer)
                    {
                        account.IsCustomerAccount = true;
                        account.RegisteredAsCustomerAt = DateTime.UtcNow;
                    }

                    if (account.IsCustomerAccount && account.IsCompanyAccount)
                    {
                        account.IsHybridAccount = true;
                    }

                    DbActionResponse dbResult = await _dbContext.ExtSaveChangesAsync();
                    
                    if (!dbResult.IsSucces)
                    {
                        response.Success = false;
                        response.APIException = dbResult.Exception;
                        response.ApiErrorCode = dbResult.ApiErrorCode;

                        result = BadRequest(response);
                    }
                    else
                    {
                        response.Success = true;
                        result = Ok(response);
                    }
                }
                else
                {
                    response.Success = false;
                    result = BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.APIException = ex.Message;
                result = BadRequest(response);
            }

            return result;
        }
    }
}
