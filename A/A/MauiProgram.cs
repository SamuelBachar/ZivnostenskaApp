using A.Interfaces;
using A.Services;
using A.Views;
using Microsoft.Extensions.Logging;

using CommunityToolkit.Maui;
using System.Net.Http;
using A.Views.LogIn;
using A.Views.Register;

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
            builder.Services.AddSingleton<IPlatformHttpMessageHandler>(sp =>
            { 
                return new A.Services.HttpClientService(); 
            });

            builder.Services.AddHttpClient(Constants.AppConstants.HttpsClientName, httpsClient =>
            {
                var baseUrl = DeviceInfo.Platform == DevicePlatform.Android
                    ? "https://10.0.2.2:7162" //7279
                    : "http://localhost:7162";

                //baseUrl = "https://inflastoreapi.azurewebsites.net/"; // TODO change

                httpsClient.BaseAddress = new Uri(baseUrl);
            }).ConfigurePrimaryHttpMessageHandler(configPrimary =>
            {
                IPlatformHttpMessageHandler? handler = configPrimary.GetService<IPlatformHttpMessageHandler>();
                return handler?.GetPlatformSpecificHttpMessageHandler();
            });

            //}).ConfigureHttpMessageHandlerBuilder(configBuilder =>
            //{
            //    var platformHttMessageHandler = configBuilder.Services.GetRequiredService<IPlatformHttpsMessageHandler>();
            //    configBuilder.PrimaryHandler = platformHttMessageHandler.GetPlatformSpecificHttpMessageHandler();
            //});

            // Services
            builder.Services.AddSingleton<ILoginService, LoginService>();

            builder.Services.AddSingleton<SettingsView>();
            builder.Services.AddSingleton<LogInView>();
            builder.Services.AddSingleton<LogInChooseView>();

            builder.Services.AddSingleton<SettingsService>();
            builder.Services.AddSingleton<RegisterCompanyView>();
            builder.Services.AddSingleton<RegisterChooseView>();

            return builder.Build();
        }
    }
}
