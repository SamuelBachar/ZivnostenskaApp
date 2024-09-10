using A.Enumerations;
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
        // Retrieve the CommandParameter, which will be of type Enums.AppMode
        string? appMode = (string)((TappedEventArgs)args).Parameter;

        // Use appMode as needed
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
}