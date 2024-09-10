using A.AppPreferences;
using A.Constants;
using A.EmbeddedResourceManager;
using A.LanguageResourceManager;
using A.Services;
using A.User;
using A.Views;
using System.Globalization;
using static A.Enumerations.Enums;

namespace A
{
    public partial class App : Application
    {
        public static LanguageManager LanguageResourceManager => LanguageManager.Instance;

        public static UserData UserData => UserData.Instance;

        public static AppMode AppMode;

        public App()
        {
            InitializeComponent();

            this.LoadSettings();

            string jsonContent = EmbeddedResource.GetEmbeddedJSONFileContent(AppConstants.JsonExceptionFilePath);

            if (jsonContent != string.Empty)
            {
                ExceptionsHandling.ExceptionHandler.DeserializeJsonExceptionFile(EmbeddedResource.GetEmbeddedJSONFileContent(AppConstants.JsonExceptionFilePath));
            }

            MainPage = new AppShell();
        }

        public void LoadSettings()
        {

            Task task = Task.Run(async () =>
            {
                await LoadDefaultUserSettings();
            });

            task.Wait(700); // safe due to timeout 700 ms, no deadlock in ctor possible
        }

        public async Task LoadDefaultUserSettings()
        {
            App.UserData.DefaultCulture = CultureInfo.CurrentCulture.Name;
            App.UserData.DefaultUICulture = CultureInfo.CurrentUICulture.Name;

            if (await SettingsService.ContainsStaticAsync(PrefUserSettings.PrefLanguage))
            {
                string storedCulture = await SettingsService.GetStaticAsync(PrefUserSettings.PrefLanguage, PrefUserSettings.PrefLanguageDefault);

                CultureInfo? culture = SettingsService.GetCultureInfo(storedCulture);

                if (culture != null)
                {
                    App.LanguageResourceManager.SetCulture(culture);
                    App.UserData.ChoosenCulture = storedCulture;
                    App.UserData.ChoosenUICulture = storedCulture;
                }
            }
            else
            {
                App.UserData.ChoosenCulture = string.Empty;
            }
        }
    }
}
