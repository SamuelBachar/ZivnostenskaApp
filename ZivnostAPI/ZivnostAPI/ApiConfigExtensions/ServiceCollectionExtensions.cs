using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedTypesLibrary.Constants;
using ZivnostAPI.Data.CusDbContext;
using ZivnostAPI.Models.AuthProvidersData;
using ZivnostAPI.Services.CompanyService;
using ZivnostAPI.Services.Generic;
using ZivnostAPI.Services.Interfaces;
using ZivnostAPI.Services.LogInService;
using ZivnostAPI.Services.OAuth;

namespace ZivnostAPI.ApiConfigClasses;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOAuthHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OAuthSettings>(configuration.GetSection("OAuth"));
        services.AddHttpClient(AuthProviders.Google, (serviceProvider, authProvider) =>
        {
            var oauthOptions = serviceProvider.GetRequiredService<IOptions<OAuthSettings>>().Value;
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

            var queryString = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={kvp.Value}"));

            authProvider.BaseAddress = new Uri($"{oauthOptions.Google.BaseUrl}/auth?{queryString}");
        });

        services.AddHttpClient(AuthProviders.Facebook, (serviceProvider, authProvider) =>
        {
            var oauthOptions = serviceProvider.GetRequiredService<IOptions<OAuthSettings>>().Value;
            var queryParams = new Dictionary<string, string>
            {
                { "response_type", "code" },
                { "client_id", $"{oauthOptions.Facebook.ClientId}" },
                { "redirect_uri",  $"{oauthOptions.RedirectUri}" },
                { "scope", "email,public_profile,user_mobile_phone" },
                { "state", $"{AuthProviders.Facebook}" }
            };

            var queryString = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={kvp.Value}"));

            authProvider.BaseAddress = new Uri($"{oauthOptions.Facebook.BaseUrl}/dialog/oauth?{queryString}");
        });

        services.AddHttpClient(AuthProviders.Apple, (serviceProvider, authProvider) =>
        {
            var oauthOptions = serviceProvider.GetRequiredService<IOptions<OAuthSettings>>().Value;
            var queryParams = new Dictionary<string, string>
            {
                { "response_type", "code" },
                { "client_id", $"{oauthOptions.Apple.ClientId}" },
                { "redirect_uri",  $"{oauthOptions.RedirectUri}" },
                { "scope", "email name" },
                { "state", $"{AuthProviders.Apple}" }
            };

            var queryString = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={kvp.Value}"));

            authProvider.BaseAddress = new Uri($"{oauthOptions.Facebook.BaseUrl}/authorize?{queryString}");
        });

        return services;
    }

    public static IServiceCollection CacheOAuthSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OAuthSettings>(configuration.GetSection("OAuth"));

        return services;
    }

    public static IServiceCollection AddOAuthUrlBuildService(this IServiceCollection services)
    {
        services.AddSingleton<OAuthUrlBuildService>(serviceProvider =>
        {
            var oauthSettings = serviceProvider.GetRequiredService<IOptions<OAuthSettings>>().Value;
            return new OAuthUrlBuildService(oauthSettings);
        });

        return services;
    }


    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericReadOnlyService<>), typeof(GenericReadOnlyService<>));
        services.AddScoped(typeof(IGenericWriteService<>), typeof(GenericWriteService<>));
        services.AddScoped(typeof(IGenericCrudService<>), typeof(GenericCrudService<>));

        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<ILogInService, LogInService>();

        return services;
    }

    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = string.Empty;

        if (System.Net.Dns.GetHostName() == "DESKTOP-DTI7TH4") // home
        {
            connectionString = configuration.GetSection("DBConnectionStrings")["DefaultConnectionHome"];
        }
        else // office
        {
            connectionString = configuration.GetSection("DBConnectionStrings")["DefaultConnectionOffice"];
        }

        if (connectionString == null)
        {
            throw new InvalidOperationException("Connection string to DB not loaded correctly");
        }
        else
        {
            services.AddDbContext<CusDbContext>(options => options.UseSqlServer(connectionString));
        }

        return services;
    }
}