using A.AppPreferences;
using A.Enumerations;
using A.Services;
using A.ViewModels;
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

public partial class LogInChooseView : ContentPage
{
    private readonly HttpClient _httpClient;

    private readonly LogInChooseViewModel _logInChooseViewModel;

    public LogInChooseView(IHttpClientFactory httpClientFactory, LogInChooseViewModel logInChooseViewModel)
	{
		InitializeComponent();

        _logInChooseViewModel = logInChooseViewModel;
        _httpClient = httpClientFactory.CreateClient(Constants.AppConstants.HttpsClientName);
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        //App.UserData.UserIdentityData.NewUser
        if (_logInChooseViewModel.NewUser)
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
            AppMode appMode = (AppMode)((TappedEventArgs)args).Parameter;

            if (appMode == AppMode.Customer)
            {
                if (_logInChooseViewModel.NewUser || !App.UserData.UserIdentityData.RegisteredAsCustomer)
                {
                    UpdateAccountTypeDTO data = new UpdateAccountTypeDTO { AccountType = AccountType.Customer };
                    ApiResponse<UpdateAccountTypeDTO> serializedResponse = await UpdateAccountType(data);

                    if (serializedResponse.Success)
                    {
                        App.AppMode = appMode;
                        await SettingsService.SavePreferedApplicationMode(appMode, this.chkDontAsk.IsChecked);
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
                    App.AppMode = appMode;
                    await SettingsService.SavePreferedApplicationMode(appMode, this.chkDontAsk.IsChecked);
                    await Shell.Current.GoToAsync($"{nameof(MainPage)}");
                }
            }

            if (appMode == AppMode.Company)
            {
                if (_logInChooseViewModel.NewUser || !App.UserData.UserIdentityData.RegisteredAsCompany)
                {
                    // When totally new user OR when user was not yet registered as company

                    // For company PreferedApplicationMode is saved after successfully registration
                    // For company UpdatingAccountType is also done after successfully registration
                    
                    App.AppMode = appMode;

                    await Shell.Current.GoToAsync(nameof(RegisterCompanyView),
                        new Dictionary<string, object>
                        {
                            ["Provider"] = _logInChooseViewModel.Provider,
                            ["IsPreferredAppModeChecked"] = this.chkDontAsk.IsChecked,
                            ["OAuthRegistration"] = _logInChooseViewModel.OAuthRegistration
                        });
                }
                else
                {
                    App.AppMode = appMode;
                    await SettingsService.SavePreferedApplicationMode(appMode, this.chkDontAsk.IsChecked);
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

    private async Task<ApiResponse<UpdateAccountTypeDTO>> UpdateAccountType(UpdateAccountTypeDTO data)
    {
        HttpResponseMessage response = await _httpClient.PutAsJsonAsync("/api/Account/UpdateAccountType", data, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        ApiResponse<UpdateAccountTypeDTO> serializedResponse = await response.Content.ExtReadFromJsonAsync<UpdateAccountTypeDTO>();

        return serializedResponse;
    }
}