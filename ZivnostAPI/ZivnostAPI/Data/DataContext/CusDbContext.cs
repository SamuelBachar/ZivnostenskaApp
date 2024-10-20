using Microsoft.EntityFrameworkCore;
using System.Drawing;
using ZivnostAPI.Models.DatabaseModels.Account;
using ZivnostAPI.Models.DatabaseModels.CompanyBaseData;
using ZivnostAPI.Models.DatabaseModels.Localization;
using Region = ZivnostAPI.Models.DatabaseModels.Localization.Region;

namespace ZivnostAPI.Data.CusDbContext;

/* TODO */

//     Entity Framework Core does not support multiple parallel operations being run
//     on the same DbContext instance. This includes both parallel execution of async
//     queries and any explicit concurrent use from multiple threads. Therefore, always
//     await async calls immediately, or use separate DbContext instances for operations
//     that execute in parallel. See Avoiding DbContext threading issues for more information
//     and examples -> https://aka.ms/efcore-docs-threading
public class CusDbContext : DbContext
{
    public CusDbContext(DbContextOptions<CusDbContext> options) : base(options)
    {
        
    }

    public DbSet<Company> Company { get; set; }
    public DbSet<Account> Account { get; set; }
    public DbSet<Country> Country { get; set; }
    public DbSet<Region> Region { get; set; }
    public DbSet<District> District { get; set; }
    public DbSet<City> City { get; set; }

}
