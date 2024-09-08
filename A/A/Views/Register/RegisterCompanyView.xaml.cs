namespace A.Views;

public partial class RegisterCompanyView : ContentPage
{
	public RegisterCompanyView()
	{
		InitializeComponent();
	}

    //https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/file-picker?view=net-maui-8.0&tabs=android
    public async Task<(FileResult?, string)> PickAndShow(PickOptions options)
    {
        FileResult? result = null;
        string msg = string.Empty;

        try
        {
            result = await FilePicker.Default.PickAsync(options);

            if (result != null)
            {
                if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                {
                    using var stream = await result.OpenReadAsync();
                    var image = ImageSource.FromStream(() => stream);
                }
            }
        }
        catch (Exception ex)
        {
            msg += ex.Message;
        }

        return (result, msg);
    }

    private async void BtnChooseImage_Clicked(object sender, EventArgs e)
    {
        await PickAndShow(PickOptions.Images);
    }
}