using Microsoft.EntityFrameworkCore;
using SharedTypesLibrary.ServiceResponseModel;
using System.Security.Cryptography.X509Certificates;
using ZivnostAPI.Data.CusDbContext;
using ZivnostAPI.Services.Interfaces;

namespace ZivnostAPI.Services.Generic
{
    public class GenericReadOnlyService<T> : IGenericReadOnlyService<T> where T : class
    {
        private readonly CusDbContext _dbContext;

        public GenericReadOnlyService(CusDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<ApiResponse<T>> GetAll()
        {
            var response = new ApiResponse<T>();

            try
            {
                response.ListData = await _dbContext.Set<T>().ToListAsync();
                response.Success = true; 
            }
            catch (Exception ex)
            {
                response.Success = false; 
                response.APIException = ex.Message;
            }

            return response;
        }

        public virtual async Task<ApiResponse<T?>> GetById(int id)
        {
            var response = new ApiResponse<T?>();

            try
            {
                var data = await _dbContext.Set<T>().FindAsync(id);
                response.Data = data;
                response.Success = data != null;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.APIException = ex.Message;
            }

            return response;
        }
    }
}
