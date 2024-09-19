using Microsoft.EntityFrameworkCore;
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

        public async Task<List<T>> GetAll()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetById(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }
    }
}
