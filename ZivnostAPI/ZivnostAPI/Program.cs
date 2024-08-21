using AvantiPoint.MobileAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using SharedTypesLibrary.Constants;
using System.Reflection;
using System.Text.Json;
using ZivnostAPI.Constants;
using ZivnostAPI.Controllers;
using ZivnostAPI.Data.DataContext;
using ZivnostAPI.Models.AuthProvidersData;
using ZivnostAPI.Services.CompanyService;
using ZivnostAPI.Services.LogInService;

var builder = WebApplication.CreateBuilder(args);

// Add Mobile Auth to builder
//builder.AddMobileAuth();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ILogInService, LogInService>();
builder.Services.AddDbContext<DataContext>();

builder.Services.Configure<OAuth>(builder.Configuration.GetSection("OAuth"));
builder.Services.AddHttpClient(AuthProviders.Google, (serviceProvider, authProvider) =>
{
    // Resolve the configured OAuth options
    var oauthOptions = serviceProvider.GetRequiredService<IOptions<OAuth>>().Value;
    var queryParams = new Dictionary<string, string>
    {
        { "response_type", "code" },
        { "client_id", $"{oauthOptions.Google.ClientId}" },
        { "redirect_uri",  $"{oauthOptions.RedirectUri}" },
        { "scope", "openid email profile" },
        { "state", $"{AuthProviders.Google}" },
        { "access_type", "offline" },
        { "prompt", "consent" }
    };

    var queryString = string.Join("&", queryParams.Select(
        kvp => $"{Uri.EscapeDataString(kvp.Key)}={kvp.Value}"
    ));

    authProvider.BaseAddress = new Uri($"{oauthOptions.Google.BaseUrl}/auth?{queryString}");
});


var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), APIConstants.JsonExceptionFilePath);
if (System.IO.File.Exists(jsonFilePath))
{
    string jsonContent = System.IO.File.ReadAllText(jsonFilePath);

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