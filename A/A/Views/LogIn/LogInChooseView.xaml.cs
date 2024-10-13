using A.AppPreferences;
using A.Enumerations;
using A.Services;
using ExceptionsHandling;
using ExtensionsLibrary.Http;
using Newtonsoft.Json;
using SharedTypesLibrary.DTOs.Bidirectional.Account;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.ServiceResponseModel;
using System.Net.Http.Json;
using System.Text.Json;
using static A.Enumerations.Enums;
using static SharedTypesLibrary.Enums.Enums;

namespace A.Views.LogIn;

[QueryProperty(nameof(NewUser), "NewUser")]
public partial class LogInChooseView : ContentPage
{
    private readonly HttpClient _httpClient;

    private bool _newUser = false;

    public bool NewUser
    {
        set
        {
            _newUser = value;
        }
    }

	public LogInChooseView(IHttpClientFactory httpClientFactory)
	{
		InitializeComponent();

        _httpClient = httpClientFactory.CreateClient(Constants.AppConstants.HttpsClientName);
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
        try
        {
            //string? appMode = (string)((TappedEventArgs)args).Parameter;

            AppMode appMode = (AppMode)((TappedEventArgs)args).Parameter;

            // Store prefered application mode if chkDontAsk is checked
            if (this.chkDontAsk.IsChecked)
            {
                SavePreferedApplicationMode(appMode);
            }

            // TODO: I had problem referencing AppMode from xaml to use it as arg in CommandParameters, therefore I am playing around with string
            if (appMode == AppMode.Company)
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

            if (appMode == AppMode.Customer)
            {
                if (_newUser)
                {
                    UpdateAccountTypeDTO data = new UpdateAccountTypeDTO { AccountType = AccountType.Customer };
                    HttpResponseMessage response = await _httpClient.PutAsJsonAsync("/api/Account/UpdateAccountType", data, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                    ApiResponse<UpdateAccountTypeDTO> serializedResponse = await response.Content.ExtReadFromJsonAsync<UpdateAccountTypeDTO>();

                    if (serializedResponse.Success)
                    {
                        await Shell.Current.GoToAsync($"{nameof(MainPage)}");
                    }
                    else
                    {
                        string errMsg = new ExceptionHandler("UAE_006", serializedResponse.ApiErrorCode, serializedResponse.APIException, App.UserData.CurrentCulture).CustomMessage;
                        await DisplayAlert(App.LanguageResourceManager["LogInChooseView_ChooseError"].ToString(), errMsg, App.LanguageResourceManager["AllView_Close"].ToString());
                    }
                }
                else
                {
                    await Shell.Current.GoToAsync($"{nameof(MainPage)}");
                }
            }
        }
        catch (Exception ex)
        {
            string exMsg = new ExceptionHandler("UAE_401", null, extraErrors: ex.Message, App.UserData.CurrentCulture).CustomMessage;
            await DisplayAlert(App.LanguageResourceManager["LogInChooseView_ChooseError"].ToString(), exMsg, App.LanguageResourceManager["AllView_Close"].ToString());
        }
    }

    private async void SavePreferedApplicationMode(AppMode appMode)
    {
        // Delete prefered App Mode if was already saved before
        if (await SettingsService.ContainsStaticAsync(PrefUserSettings.AppModeChoice))
        {
            await SettingsService.RemoveStaticAsync(PrefUserSettings.AppModeChoice);
        }

        // Update prefered App Mode based on login choose
        /*if (appMode == "Customer")
        {
            await SettingsService.SaveStaticAsync<AppMode>(PrefUserSettings.AppModeChoice, AppMode.Customer);
        }
        else
        {
            await SettingsService.SaveStaticAsync<AppMode>(PrefUserSettings.AppModeChoice, AppMode.Company);
        }*/

        await SettingsService.SaveStaticAsync<AppMode>(PrefUserSettings.AppModeChoice, appMode);
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