﻿using A.Interfaces;
using A.Services;
using A.Views;
using Microsoft.Extensions.Logging;

using CommunityToolkit.Maui;
using System.Net.Http;
using A.Views.LogIn;
using A.Views.Register;
using CustomUIControls.Interfaces;
using A.CustomControls.CustomControlsDefines.EndpointDefines;
using A.CustomControls.CustomControlsDefines.RelationshipDefines;
using A.ViewModels;

using CustomControlsLibrary.Interfaces;
#if ANDROID
using CustomControlsLibrary.Platforms.Droid;
#elif IOS
using CustomControlsLibrary.Platforms.iOS;
#endif

namespace A
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            // HTTPS client
            builder.Services.AddSingleton<IPlatformHttpMessageHandler, HttpClientService>(); // Register service for platform specific HttpHandlers
            builder.Services.AddTransient<ConnectivityHandler>(); // Adding ConnectivityHandler as delegating handler

            builder.Services.AddHttpClient(Constants.AppConstants.HttpsClientName, httpsClient =>
            {
                var baseUrl = DeviceInfo.Platform == DevicePlatform.Android
                    ? "https://10.0.2.2:7162"
                    : "http://localhost:7162";

                //baseUrl = "https://inflastoreapi.azurewebsites.net/"; // TODO change

                httpsClient.BaseAddress = new Uri(baseUrl);
            }).ConfigurePrimaryHttpMessageHandler(configPrimary =>
            {
                var handler = configPrimary.GetRequiredService<IPlatformHttpMessageHandler>();
                return handler.GetPlatformSpecificHttpMessageHandler();
            })
            .AddHttpMessageHandler<ConnectivityHandler>(); // Pridanie ConnectivityHandler do reťazca

            // LogInView
            builder.Services.AddSingleton<ILoginService, LoginService>();
            builder.Services.AddSingleton<IOauthService, OauthService>();

            // RegisterCompanyView
            builder.Services.AddSingleton<RegisterCompanyView>();
            builder.Services.AddSingleton<IEndpointResolver, CustomEndpointDefines>();
            builder.Services.AddSingleton<IRelationshipResolver, CustomRelationshipDefines>();

            builder.Services.AddSingleton<SettingsView>();
            builder.Services.AddSingleton<LogInView>();
            builder.Services.AddSingleton<LogInChooseView>();

            builder.Services.AddSingleton<SettingsService>();
            builder.Services.AddSingleton<RegisterChooseView>();

            // ViewModels
            builder.Services.AddTransient<RegisterCompanyViewModel>();
            builder.Services.AddScoped<LogInChooseViewModel>();

#if ANDROID || IOS
            builder.Services.AddSingleton<IDisplayService, DisplayService>();
#endif

            return builder.Build();
        }
    }
}
