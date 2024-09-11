using System.Globalization;

namespace A.Views
{

    [QueryProperty(nameof(GenericRegistration), "genericRegistration")]
    public partial class RegisterCompanyView : ContentPage
    {
        private int _viewIndex = 0;
        public int ViewIndex
        {
            get => _viewIndex;
            set
            {
                _viewIndex = value;
                OnPropertyChanged(nameof(ViewIndex)); // Notify the UI about the change
            }
        }

        ImageSource? _imageSource { get; set; } = null;
        bool _genericRegistration { get; set; } = false;

        Grid[] _arrGridViews = new Grid[3];

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
            this.BindingContext = this;

            //_arrGridViews[0] = this.FirstStepView;
            //_arrGridViews[1] = this.SecondStepView;
            //_arrGridViews[2] = this.ThirdStepView;

            //_arrGridViews[_viewIndex].IsVisible = true;

            if (this._genericRegistration)
            {
                this.lblTitleViewStep.Text = "1/3";
            }
            else
            {
                this.lblTitleViewStep.Text = "1/2";
            }

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
            if (_viewIndex < 2) // Prevent going beyond available views
            {
                ViewIndex++;
                lblTitleViewStep.Text = $"{ViewIndex + 1}/{(_genericRegistration ? 3 : 2)}";
            }
        }

        private void BtnPrev_Clicked(object sender, EventArgs e)
        {
            if (_viewIndex > 0) // Prevent negative index
            {
                ViewIndex--;
                lblTitleViewStep.Text = $"{ViewIndex + 1}/{(_genericRegistration ? 3 : 2)}";
            }
        }

        private void BtnRegister_Clicked(object sender, EventArgs e)
        {

        }

        private void EntryEmail_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void EntryPassword_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void EntryPasswordConfirm_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

namespace A.Converters
{
    public class StepVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int viewIndex && parameter is string gridIndex)
            {
                // Parse the gridIndex passed as a parameter in XAML and compare it with _viewIndex
                return viewIndex == int.Parse(gridIndex);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}