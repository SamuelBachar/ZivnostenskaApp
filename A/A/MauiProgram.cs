using A.Interfaces;
using A.Services;
using A.Views;
using Microsoft.Extensions.Logging;

using CommunityToolkit.Maui;

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

            builder.Services.AddSingleton<SettingsView>();
            builder.Services.AddSingleton<SettingsService>();

            return builder.Build();
        }
    }
}
