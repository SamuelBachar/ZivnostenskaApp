namespace A.Views;


[QueryProperty(nameof(GenericRegistration), "genericRegistration")]
public partial class RegisterCompanyView : ContentPage
{
    int _viewIndex = 0;
    ImageSource? _imageSource { get; set; } = null;
    bool _genericRegistration { get; set; } = false;

    public bool GenericRegistration
    {
        set
        {
            _genericRegistration = value;
        }
    }

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
                    _imageSource = ImageSource.FromStream(() => stream);

                    this.ImgCompanyLogo.Source = _imageSource;
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

    private void BtnNext_Clicked(object sender, EventArgs e)
    {
        _viewIndex++;
    }

    private void BtnPrev_Clicked(object sender, EventArgs e)
    {
        _viewIndex--;
    }
}