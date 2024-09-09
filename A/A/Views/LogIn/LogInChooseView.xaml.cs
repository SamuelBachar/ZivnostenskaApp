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
        string? appMode = args.Parameter as string;

        // navigate to corret home page based on certain app Mode
        // or if user is new and Company was choosed than proceed with registration

        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
    }
}