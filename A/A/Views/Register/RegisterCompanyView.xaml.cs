using System.Globalization;
using A.Interfaces;
using SharedTypesLibrary.DTOs.Request;
using A.CustomControls.Controls;
using CustomUIControls.Interfaces;
using ExceptionsHandling;
using System.Net.Http;

using SharedTypesLibrary.DTOs.Bidirectional.Localization;
using SharedTypesLibrary.DTOs.Bidirectional.Services;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ExtensionsLibrary.Http;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.ServiceResponseModel;
using A.ViewModels;
using CustomControlsLibrary.Interfaces;
using System.ComponentModel;

namespace A.Views
{
    public partial class RegisterCompanyView : BasePage
    {
        bool _genericRegistration = false;
        bool _oAuthRegistration = false;

        public bool OAuthRegistration
        {
            get => _oAuthRegistration;
            set
            {
                _oAuthRegistration = value;
                OnPropertyChanged(nameof(OAuthRegistration));
            }
        }

        bool _isPreferredAppModeChecked { get; set; } = false;


        private int _viewIndex = 0;
        public int ViewIndex
        {
            get => _viewIndex;
            set
            {
                _viewIndex = value;
                OnPropertyChanged(nameof(ViewIndex));
            }
        }

        private string _provider = string.Empty;

        private class ImageData
        {
            public ImageSource? ImageSource { get; set; } = null;
            public Stream? ImageStream { get; set; } = null;

            public string FileName { get; set; } = string.Empty;
        }

        ImageData? _imageData { get; set; } = null;

        private HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IEndpointResolver _endpointResolver;
        private readonly IRelationshipResolver _relationshipResolver;
        private readonly IDisplayService _displayService;
        private readonly RegisterCompanyViewModel _registerCompanyViewModel;
        public RegisterCompanyView(IHttpClientFactory httpClientFactory, IEndpointResolver endpointResolver,
                                   IRelationshipResolver relationshipResolver, IDisplayService displayService, RegisterCompanyViewModel registerCompanyViewModel)
        {
            InitializeComponent();

            _endpointResolver = endpointResolver;
            _relationshipResolver = relationshipResolver;
            _httpClientFactory = httpClientFactory;
            _registerCompanyViewModel = registerCompanyViewModel;
            _displayService = displayService;

            this.Loaded += async (s, e) => { await LoadData(); };
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            _oAuthRegistration = _registerCompanyViewModel.OAuthRegistration;
            _isPreferredAppModeChecked = _registerCompanyViewModel.IsPreferredAppModeChecked;
            _provider = _registerCompanyViewModel.Provider;

            OAuthRegistration = _oAuthRegistration;
            _genericRegistration = !_oAuthRegistration;

            this.lblTitleViewStep.Text = "1/4";

            if (_oAuthRegistration)
            {
                this.LblFourthStep.Text = "Zadajte alternatÌvne prihlasovacie ˙daje";
                this.LblFourthStep.Text += "Zad·vanie ˙dajov nie je povinnÈ, naÔalej mÙûte vyuûÌvaù #provider pre prihlasovanie";
                this.LblFourthStep.Text = this.LblFourthStep.Text.Replace("%#provider", _provider);
                this.BtnRegister4Skip.IsVisible = true;
            }
            else
            {
                this.LblFourthStep.Text = "Zadajte prihlasovacie ˙daje";
                this.BtnRegister4Skip.IsVisible = false;
                this.EntryEmailRegister.IsMandatory = true;
                this.EntryPassword.IsMandatory = true;
                this.EntryPasswordConfirm.IsMandatory = true;

                OAuthRegistration = true; // test - DO NOT WORK really
            }

            this.BindingContext = this;
        }


        private async Task LoadData()
        {
            try
            {
                _httpClient = _httpClientFactory.CreateClient(Constants.AppConstants.HttpsClientName);

                base.SetIsBusy(true);
                await this.RegionPicker.Initialize(_httpClient, _endpointResolver, _relationshipResolver, typeof(RegionDTO));
                await this.DistrictPicker.Initialize(_httpClient, _endpointResolver, _relationshipResolver, typeof(DistrictDTO));
                await this.CityEntryPicker.Initialize(_httpClient, _endpointResolver, _relationshipResolver, _displayService, typeof(CityDTO));
                await this.ServiceCategoryList.Initialize(_httpClient, _endpointResolver, _displayService);
            }
            catch (Exception ex)
            {
                string errMsg = new ExceptionHandler("UAE_401", null, extraErrors: ex.Message, App.UserData.CurrentCulture).CustomMessage;
                await DisplayAlert(App.LanguageResourceManager["RegisterCompanyView_RegisterError"].ToString(), errMsg, App.LanguageResourceManager["AllView_Close"].ToString());
            }
            finally
            {
                base.SetIsBusy(false);
            }
        }

        private void UpdateViewIndexAndRegistration()
        {
            OnPropertyChanged(nameof(ViewIndex));
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
                        if (_imageData != null && _imageData.ImageStream != null)
                        {
                            await _imageData.ImageStream.DisposeAsync();
                        }
                        else
                        {
                            _imageData = new ImageData();
                        }

                        _imageData.ImageStream = await result.OpenReadAsync();
                        _imageData.ImageSource = ImageSource.FromStream(() => _imageData.ImageStream);

                        //this.ImgCompanyLogo.Source = _imageData.ImageSource;

                        _imageData.FileName = result.FileName;
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
            //if (ValidateData(_viewIndex))
            //{
            if (_viewIndex < 3) // Prevent going beyond available views
            {
                ViewIndex++;
                lblTitleViewStep.Text = $"{ViewIndex + 1}/4";
            }
            // }
        }

        private void BtnPrev_Clicked(object sender, EventArgs e)
        {
            if (_viewIndex > 0) // Prevent negative index
            {
                ViewIndex--;
                lblTitleViewStep.Text = $"{ViewIndex + 1}/4";
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

        private string GetImageFormat(string fileName)
        {
            string result = string.Empty;

            string fileExtension = Path.GetExtension(fileName).ToLower();

            if (fileExtension == ".png")
            {
                result = "image/png";
            }
            else if (fileExtension == ".jpg" || fileExtension == ".jpeg")
            {
                result = "image/jpeg";
            }
            else
            {
                throw new InvalidOperationException("Unsupported image format");
            }

            return result;
        }

        private async void BtnRegister_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (ValidateData(_viewIndex))
                {
                    RegistrationCompanyRequest regCompData = new RegistrationCompanyRequest
                    {
                        Id = App.UserData.UserIdentityData.Id,
                        CompanyName = this.EntryCompany.Text,
                        CIN = this.EntryCIN.Text,
                        Address = this.EntryAddress.Text,
                        PostalCode = this.EntryPostalCode.Text,
                        //CompanyDescription = this.EditorCompanyDescription.Text,

                        // required
                        Email = this.EntryEmail.Text,
                        Phone = this.EntryPhone.Text,
                        DistrictCompany = (DistrictDTO)this.DistrictPicker.SelectedItem,
                        RegionCompany = (RegionDTO)this.DistrictPicker.SelectedItem,
                        City = (CityDTO)this.CityEntryPicker.SelectedItem,
                        Country = new CountryDTO { Name = "test" },
                        ListServices = new List<ServiceDTO>()
                    };

                    if (_oAuthRegistration)
                    {
                        RegisterGenericCredentials regGenData = new RegisterGenericCredentials
                        {
                            Email = this.EntryEmailRegister.Text,
                            Password = this.EntryPassword.Text,
                            PasswordConfirmed = this.EntryPasswordConfirm.Text
                        };
                    }
                    else
                    {
                        // Use Auth Provider data
                    }

                    using MultipartFormDataContent content = new MultipartFormDataContent();

                    content.Add(new StringContent(regCompData.Id.ToString()), nameof(RegistrationCompanyRequest.Id));
                    content.Add(new StringContent(regCompData.CompanyName), nameof(RegistrationCompanyRequest.CompanyName));
                    content.Add(new StringContent(regCompData.CIN), nameof(RegistrationCompanyRequest.CIN));
                    content.Add(new StringContent(regCompData.Phone), nameof(RegistrationCompanyRequest.Phone));
                    content.Add(new StringContent(regCompData.Email), nameof(RegistrationCompanyRequest.Email));
                    content.Add(new StringContent(regCompData.Address), nameof(RegistrationCompanyRequest.Address));
                    content.Add(new StringContent(regCompData.PostalCode), nameof(RegistrationCompanyRequest.PostalCode));
                    content.Add(new StringContent(regCompData.CompanyDescription), nameof(RegistrationCompanyRequest.CompanyDescription));
                    content.Add(new StringContent(JsonConvert.SerializeObject(regCompData.Country)), nameof(RegistrationCompanyRequest.Country));
                    content.Add(new StringContent(JsonConvert.SerializeObject(regCompData.City)), nameof(RegistrationCompanyRequest.City));
                    content.Add(new StringContent(JsonConvert.SerializeObject(regCompData.RegionCompany)), nameof(RegistrationCompanyRequest.RegionCompany));
                    content.Add(new StringContent(JsonConvert.SerializeObject(regCompData.DistrictCompany)), nameof(RegistrationCompanyRequest.DistrictCompany));

                    content.Add(new StringContent(JsonConvert.SerializeObject(regCompData.ListServices)), nameof(RegistrationCompanyRequest.ListServices));

                    // Add the image
                    if (_imageData != null && _imageData.ImageStream != null)
                    {
                        _imageData.ImageStream.Position = 0;

                        var streamContent = new StreamContent(_imageData.ImageStream);
                        var imageContent = new ByteArrayContent(regCompData.Image);

                        imageContent.Headers.ContentType = new MediaTypeHeaderValue(GetImageFormat(_imageData.FileName));

                        content.Add(imageContent, "Image", $"{_imageData.FileName}");
                    }

                }
            }
            catch (Exception ex)
            {
            }
            finally
            {



            }
        }


        private bool ValidateData(int _viewIndex)
        {
            bool result = true;

            if (_viewIndex == 0)
            {
                result = this.EntryPhone.Validate(checkLength: true) & this.EntryEmail.Validate(checkLength: true) &
                         this.RegionPicker.Validate() & this.DistrictPicker.Validate() & this.CityEntryPicker.Validate() &
                         this.EntryCompany.Validate(checkLength: true);
            }

            if (_viewIndex == 1)
            {
                // todo check if some services are choosed if not return false

                result = false;
            }

            if (_viewIndex == 2)
            {
                // TODO: most likely not needed to check logo of company or describtion of company

                result = false;
            }

            if (_viewIndex == 3)
            {
                result = this.EntryPassword.Validate(checkLength: true) & this.EntryPasswordConfirm.Validate(checkLength: true) & 
                         EntryEmailRegister.Validate(checkLength: true);

                if (_genericRegistration)
                {
                    if (result &&
                        (!string.IsNullOrEmpty(this.EntryPassword.Text) || !string.IsNullOrEmpty(this.EntryPasswordConfirm.Text)) &&
                        this.EntryPassword.Text != this.EntryPasswordConfirm.Text)
                    {
                        result = false;

                        string errMsg = App.LanguageResourceManager["Controls_CustomEntry_Password_PasswordsNotSame"].ToString() ?? "Error passwords are not matching";

                        this.EntryPassword.ErrorMessage = errMsg;
                        this.EntryPasswordConfirm.ErrorMessage = errMsg;
                    }
                }


            }

            return result;
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
                // Show only specific grid
                return viewIndex == int.Parse(gridIndex);
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Ensure the parameter is a format string
            if (parameter is string format && value != null)
            {
                return string.Format(format, value);
            }
            return parameter; // If no value, return the unformatted string
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Not needed for one-way binding
        }
    }
}
