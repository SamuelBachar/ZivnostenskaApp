using System.Globalization;
using A.Interfaces;
using SharedTypesLibrary.DTOs.Request;
using A.CustomControls.Controls;
using CustomUIControls.Interfaces;
using ExceptionsHandling;
using System.Net.Http;

using SharedTypesLibrary.DTOs.Bidirectional.Localization;

namespace A.Views
{

    [QueryProperty(nameof(GenericRegistration), "genericRegistration")]
    public partial class RegisterCompanyView : BasePage
    {

        private int _viewIndex = 0;
        public int ViewIndex
        {
            get => _viewIndex;
            set
            {
                _viewIndex = value;
                OnPropertyChanged(nameof(ViewIndex)); // Notify the UI about the change
                UpdateViewIndexAndRegistration();
            }
        }

        bool _genericRegistration { get; set; } = false;

        public bool GenericRegistration
        {
            set
            {
                _genericRegistration = value;
                UpdateViewIndexAndRegistration();
            }
        }

        public Tuple<int, bool> ViewIndexAndRegistration => new Tuple<int, bool>(ViewIndex, _genericRegistration);


        ImageSource? _imageSource { get; set; } = null;

        RegistrationCompanyDataRequest _regCompData = new RegistrationCompanyDataRequest();

        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IEndpointResolver _endpointResolver;
        private readonly IRelationshipResolver _relationshipResolver;

        public RegisterCompanyView(IHttpClientFactory httpClientFactory, IEndpointResolver endpointResolver, IRelationshipResolver relationshipResolver)
        {
            InitializeComponent();
            //this.BindingContext = this;

            _endpointResolver = endpointResolver;
            _relationshipResolver = relationshipResolver;
            _httpClientFactory = httpClientFactory;

            _httpClient = _httpClientFactory.CreateClient(Constants.AppConstants.HttpsClientName);

            this.Loaded += async (s, e) => { await LoadData(); };
        }

        private async Task LoadData()
        {
            try
            {
                base.SetIsBusy(true);
                await this.RegionPicker.Initialize(_httpClient, _endpointResolver, _relationshipResolver, typeof(RegionDTO));
                await this.DistrictPicker.Initialize(_httpClient, _endpointResolver, _relationshipResolver, typeof(DistrictDTO));
            }
            catch (Exception ex)
            {
                string errMsg = new ExceptionHandler("UAE_901", extraErrors: ex.Message, App.UserData.CurrentCulture).CustomMessage;
                await DisplayAlert(App.LanguageResourceManager["RegisterCompanyView_RegisterError"].ToString(), errMsg, App.LanguageResourceManager["AllView_Close"].ToString());
            }
            finally
            {
                base.SetIsBusy(false);
            }
        }

        private void UpdateViewIndexAndRegistration()
        {
            OnPropertyChanged(nameof(ViewIndexAndRegistration));
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

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            if (this._genericRegistration)
            {
                this.lblTitleViewStep.Text = "1/3";
            }
            else
            {
                this.lblTitleViewStep.Text = "1/2";
            }
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

        private void RegionPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Picker regionPicker = (Picker)sender;

            //int selectedIndex = regionPicker.SelectedIndex;

            //if (selectedIndex != -1)
            //{
            //    this.DistrictPicker.ItemsSource = _dicDistrict[selectedIndex];

            //    Region choosenRegion = (Region)this.RegionPicker.SelectedItem;
            //    _regCompData.RegionCompany = choosenRegion;
            //}
        }

        private void District_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Picker districtPicker = (Picker)sender;

            //int selectedIndex = districtPicker.SelectedIndex;

            //if (selectedIndex != -1)
            //{
            //    District choosenDistrict = (District)this.DistrictPicker.SelectedItem;
            //    _regCompData.DistrictCompany = choosenDistrict;
            //}
        }

        private void EntryAddress_Completed(object sender, EventArgs e)
        {
            var entry = (Entry)sender;
            _regCompData.Address = entry.Text;
        }

        private void EntryAddress_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            _regCompData.Address = entry.Text;
        }

        private void EntryPostalCode_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            _regCompData.PostalCode = entry.Text;
        }

        private void EntryPostalCode_Completed(object sender, EventArgs e)
        {
            var entry = (Entry)sender;
            _regCompData.PostalCode = entry.Text;
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
            if (value is Tuple<int, bool> viewIndexAndRegistration && parameter is string gridIndex)
            {
                int viewIndex = viewIndexAndRegistration.Item1;
                bool genericRegistration = viewIndexAndRegistration.Item2;

                if (gridIndex == "2")
                {
                    // Only show the third grid if ViewIndex is 2 and GenericRegistration is true
                    return viewIndex == 2 && genericRegistration;
                }
                // For other grids, only compare with viewIndex
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