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

	public LogInChooseView(bool newUser)
	{
        _newUser = newUser;
		InitializeComponent();
	}

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);

        if (_newUser)
        {
            this.LblInfo.Text = App.LanguageResourceManager["LogInChooseView_NewUserContinueAs"].ToString();
        }
        else
        {
            this.LblInfo.Text = App.LanguageResourceManager["LogInChooseView_KnownUserContinueAs"].ToString();
        }
    }

}