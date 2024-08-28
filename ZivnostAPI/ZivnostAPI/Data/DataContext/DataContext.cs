using Microsoft.EntityFrameworkCore;
using ZivnostAPI.Models.Account;
using ZivnostAPI.Models.CompanyBaseData;

namespace ZivnostAPI.Data.DataContext;

/* TODO */

//     Entity Framework Core does not support multiple parallel operations being run
//     on the same DbContext instance. This includes both parallel execution of async
//     queries and any explicit concurrent use from multiple threads. Therefore, always
//     await async calls immediately, or use separate DbContext instances for operations
//     that execute in parallel. See Avoiding DbContext threading issues for more information
//     and examples -> https://aka.ms/efcore-docs-threading
public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        if (System.Net.Dns.GetHostName() == "DESKTOP-DTI7TH4") // home
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=ZivnostAppDB;Trusted_Connection=true;TrustServerCertificate=true;");
        else // office
            optionsBuilder.UseSqlServer("Server=.\\SQLSERVER;Database=ZivnostAppDB;Trusted_Connection=true;TrustServerCertificate=true;");
    }

    public DbSet<Company> Company { get; set; }
    public DbSet<Account> Account { get; set; }
}
