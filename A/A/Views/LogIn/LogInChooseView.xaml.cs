using A.AppPreferences;
using A.Enumerations;
using A.Services;
using Newtonsoft.Json;
using static A.Enumerations.Enums;

namespace A.Views.LogIn;

[QueryProperty(nameof(NewUser), "newuser")]
public partial class LogInChooseView : ContentPage
{
    private bool _newUser = false;

    public bool NewUser
    {
        set
        {
            _newUser = value;
        }
    }

	public LogInChooseView()
	{
		InitializeComponent();
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (_newUser)
        {
            this.LblInfo.Text = App.LanguageResourceManager["LogInChooseView_NewUserContinueAs"].ToString();
        }
        else
        {
            this.LblInfo.Text = App.LanguageResourceManager["LogInChooseView_KnownUserContinueAs"].ToString();
        }
    }

    private async void OnChooseAppMode_Tapped(object sender, TappedEventArgs args)
    {
        string? appMode = (string)((TappedEventArgs)args).Parameter;

        // Store prefered application mode if chkDontAsk is checked
        if (this.chkDontAsk.IsChecked)
        {
           SavePreferedApplicationMode(appMode);
        }

        // TODO: I had problem referencing AppMode from xaml to use it as arg in CommandParameters, therefore I am playing around with string
        if (appMode == "Company")
        {
            _newUser = true;
            if (_newUser)
            {
                await Shell.Current.GoToAsync($"{nameof(RegisterCompanyView)}");
            }
            else
            {
                await Shell.Current.GoToAsync($"{nameof(MainPage)}");
            }
        }

        if (appMode == "Customer")
        {
            await Shell.Current.GoToAsync($"{nameof(MainPage)}");
        }
    }

    private async void SavePreferedApplicationMode(string appMode)
    {
        // Delete prefered App Mode if was already saved before
        if (await SettingsService.ContainsStaticAsync(PrefUserSettings.AppModeChoice))
        {
            await SettingsService.RemoveStaticAsync(PrefUserSettings.AppModeChoice);
        }

        // Update prefered App Mode based on login choose
        if (appMode == "Customer")
        {
            await SettingsService.SaveStaticAsync<AppMode>(PrefUserSettings.AppModeChoice, AppMode.Customer);
        }
        else
        {
            await SettingsService.SaveStaticAsync<AppMode>(PrefUserSettings.AppModeChoice, AppMode.Company);
        }
    }

    private async void chkDontAsk_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (await SettingsService.ContainsStaticAsync(PrefUserSettings.PrefRememberAppModeChoice))
        {
            await SettingsService.RemoveStaticAsync(PrefUserSettings.PrefRememberAppModeChoice);
        }

        await SettingsService.SaveStaticAsync<bool>(PrefUserSettings.PrefRememberAppModeChoice, e.Value);
    }
}