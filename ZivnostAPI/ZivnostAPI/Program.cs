using AvantiPoint.MobileAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Reflection;
using System.Text.Json;
using ZivnostAPI.Constants;
using ZivnostAPI.Data.DataContext;
using ZivnostAPI.Services.CompanyService;

var builder = WebApplication.CreateBuilder(args);

// Add Mobile Auth to builder
//builder.AddMobileAuth();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddDbContext<DataContext>();

var dbContextTypes = Assembly.GetExecutingAssembly()
                             .GetTypes()
                             .Where(t => t.IsSubclassOf(typeof(DbContext)));

Console.WriteLine("Found DbContext classes:");
foreach (var dbContextType in dbContextTypes)
{
    Console.WriteLine(dbContextType.Name);
}

var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), APIConstants.JsonExceptionFilePath);
if (File.Exists(jsonFilePath))
{
    string jsonContent = File.ReadAllText(jsonFilePath);

    if (jsonContent != string.Empty)
    {
        ExceptionsHandling.ExceptionHandler.DeserializeJsonExceptionFile(jsonContent);
    }
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAuthentication();

// maps https://{host}/mobileauth/{Apple|Google|Microsoft}

//app.MapMobileAuthRoute();

app.MapControllers();

app.Run();


// tu pozriet
//https://youtu.be/8pH5Lv4d5-g?t=4167