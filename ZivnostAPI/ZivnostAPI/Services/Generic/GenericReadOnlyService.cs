using Microsoft.EntityFrameworkCore;
using SharedTypesLibrary.ServiceResponseModel;
using System.Security.Cryptography.X509Certificates;
using ZivnostAPI.Data.CusDbContext;

namespace ZivnostAPI.Services.Generic
{
    public class GenericReadOnlyService<T> : IReadOnlyService<T> where T : class
    {
        private readonly CusDbContext _dbContext;

        public GenericReadOnlyService(CusDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<List<T>>> GetAll()
        {
            var response = new ApiResponse<List<T>>();

            try
            {
                response.Data = await _dbContext.Set<T>().ToListAsync();
                response.Success = true; 
            }
            catch (Exception ex)
            {
                response.Success = false; 
                response.ExceptionMessage = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<T?>> GetById(int id)
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
                response.ExceptionMessage = ex.Message;
            }

            return response;
        }
    }
}
