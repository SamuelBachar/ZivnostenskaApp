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
            get => _genericRegistration;
            set
            {
                _genericRegistration = value;
                UpdateViewIndexAndRegistration();
            }
        }

        public Tuple<int, bool> ViewIndexAndRegistration => new Tuple<int, bool>(ViewIndex, _genericRegistration);

        ImageSource? _imageSource { get; set; } = null;

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
            if (ValidateData(_viewIndex))
            {
                if (_viewIndex < 2) // Prevent going beyond available views
                {
                    ViewIndex++;
                    lblTitleViewStep.Text = $"{ViewIndex + 1}/{(_genericRegistration ? 3 : 2)}";
                }
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

        private async void BtnRegister_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (_genericRegistration)
                {
                    RegisterGenericRequest reqRegister = new RegisterGenericRequest
                    {
                        Email = this.EntryEmailRegister.Text,
                        Password = this.EntryPassword.Text,
                        PasswordConfirmed = this.EntryPasswordConfirm.Text
                    };

                    HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/RegisterGeneric", reqRegister, options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    ApiResponse<RegisterGenericResponse> responseData = await response.Content.ExtReadFromJsonAsync<RegisterGenericResponse>();

                    if (!responseData.Success)
                    {

                    }
                    else
                    {

                    }
                }


                using MultipartFormDataContent content = new MultipartFormDataContent();

                RegistrationCompanyDataRequest regCompData = new RegistrationCompanyDataRequest
                {
                    CompanyName = this.EntryCompany.Text,
                    CIN = this.EntryCIN.Text,
                    Address = this.EntryAddress.Text,
                    PostalCode = this.EntryPostalCode.Text,
                    CompanyDescription = this.EditorCompanyDescription.Text,

                    // required
                    Email = this.EntryEmail.Text,
                    Phone = this.EntryPhone.Text,
                    DistrictCompany = (DistrictDTO)this.DistrictPicker.SelectedItem,
                    RegionCompany = (RegionDTO)this.DistrictPicker.SelectedItem,
                    City = new CityDTO { District_Id = 1, Name = "test" },
                    Country = new CountryDTO { Name = "test" },
                    ListServices = new List<ServiceDTO>()
                };

                // Add text fields
                content.Add(new StringContent(regCompData.CompanyName), nameof(RegistrationCompanyDataRequest.CompanyName));
                content.Add(new StringContent(regCompData.CIN), nameof(RegistrationCompanyDataRequest.CIN));
                content.Add(new StringContent(regCompData.Phone), nameof(RegistrationCompanyDataRequest.Phone));
                content.Add(new StringContent(regCompData.Email), nameof(RegistrationCompanyDataRequest.Email));
                content.Add(new StringContent(regCompData.Address), nameof(RegistrationCompanyDataRequest.Address));
                content.Add(new StringContent(regCompData.PostalCode), nameof(RegistrationCompanyDataRequest.PostalCode));
                content.Add(new StringContent(JsonConvert.SerializeObject(regCompData.Country)), nameof(RegistrationCompanyDataRequest.Country));
                content.Add(new StringContent(JsonConvert.SerializeObject(regCompData.City)), nameof(RegistrationCompanyDataRequest.City));
                content.Add(new StringContent(JsonConvert.SerializeObject(regCompData.RegionCompany)), nameof(RegistrationCompanyDataRequest.RegionCompany));
                content.Add(new StringContent(JsonConvert.SerializeObject(regCompData.DistrictCompany)), nameof(RegistrationCompanyDataRequest.DistrictCompany));

                // Add List of Services as JSON
                content.Add(new StringContent(JsonConvert.SerializeObject(regCompData.ListServices)), nameof(RegistrationCompanyDataRequest.ListServices));

                // Add the image
                if (regCompData.Image != null)
                {
                    var imageContent = new ByteArrayContent(regCompData.Image);
                    imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/png"); // or "image/jpeg"
                    content.Add(imageContent, "Image", "companylogo.png");
                }

                // Send the request
                //var response = await _httpClient.PostAsync("/api/endpoint", content);


                if (_genericRegistration)
                {

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
                result = this.EntryPhone.Validate(checkLength: true) && this.EntryEmail.Validate(checkLength: true);
            }

            if (_viewIndex == 1)
            {
                // todo check if some services are choosed if not return false

                result = false;
            }

            if (_viewIndex == 2)
            {
                // todo check email entry and passwords an validate ... if not correct mail or passwords are not same cancel registration

                result = false;
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