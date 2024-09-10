namespace A.Views.Register;

public partial class RegisterChooseView : ContentPage
{
	public RegisterChooseView()
	{
		InitializeComponent();
	}

	private async void OnRegisterMode_Tapped(object sender, TappedEventArgs args)
	{
        string? appMode = (string)((TappedEventArgs)args).Parameter;

        // TODO: I had problem referencing AppMode from xaml to use it as arg in CommandParameters, therefore I am playing around with string
        if (appMode == "Company")
        {
            await Shell.Current.GoToAsync($"{nameof(RegisterCompanyView)}?genericRegistration={true}");
        }

        if (appMode == "Customer")
        {
            await Shell.Current.GoToAsync($"{nameof(MainPage)}");
        }
    }
}