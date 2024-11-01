using A.ViewModels;
using CustomControlsLibrary.Interfaces;
using CustomUIControls.Interfaces;
using ExtensionsLibrary.Http;
using SharedTypesLibrary.DTOs.Bidirectional.Categories;
using SharedTypesLibrary.DTOs.Bidirectional.Services;
using SharedTypesLibrary.ServiceResponseModel;
using System.Collections.ObjectModel;

using static CustomUIControls.Enumerations.Enums;

namespace A.CustomControls.Controls;

public partial class ServiceCategoryList : ContentView
{
    private ObservableCollection<ServiceDTO> _collServices = new ObservableCollection<ServiceDTO>();
    private ObservableCollection<CategoryViewModel> _collCategories = new ObservableCollection<CategoryViewModel>();
    private ObservableCollection<CategoryViewModel> _collChosenCategories = new ObservableCollection<CategoryViewModel>();

    private HttpClient _httpClient;
    private IEndpointResolver _endpointResolver;
    IDisplayService _displayService;
    public ServiceCategoryList()
	{
        InitializeComponent();
    }

    public async Task Initialize(HttpClient httpClient, IEndpointResolver endpointResolver, IDisplayService displayService)
    {
        _httpClient = httpClient;
        _endpointResolver = endpointResolver;
        _displayService = displayService;

        await LoadData();

        ServiceCollectionView.ItemsSource = _collServices;
        CategoryCollectionView.ItemsSource = _collCategories;
        ChosenCategoriesCollectionView.ItemsSource = _collChosenCategories;

        var (screenWidth, screenHeight) = _displayService.GetScreenSizes();
        var (finalWidth, finalHeight) = _displayService.GetFinalLayoutSize((screenWidth, screenHeight), scaleW: 0.50, scaleH: 0.75);

        this.MaximumHeightRequest = finalHeight;
    }

    private async Task LoadData()
    {
        if (_httpClient == null || _endpointResolver == null)
            return;

        string endPointServices = _endpointResolver.GetEndpoint<ServiceDTO>(ApiAction.GetAll);
        var response = await _httpClient.GetAsync(endPointServices);
        response.EnsureSuccessStatusCode();
        ApiResponse<ServiceDTO> apiResponseSer = await response.Content.ExtReadFromJsonAsync<ServiceDTO>();

        apiResponseSer.ListData?.ToList().ForEach(data => _collServices.Add(data));

        string endPointCategories = _endpointResolver.GetEndpoint<CategoryDTO>(ApiAction.GetAll);
        response = await _httpClient.GetAsync(endPointCategories);
        response.EnsureSuccessStatusCode();
        ApiResponse<CategoryDTO> apiResponseCat = await response.Content.ExtReadFromJsonAsync<CategoryDTO>();

        apiResponseCat.ListData?.ToList().ForEach(data => _collCategories.Add( new CategoryViewModel(data)));
    }

    private void OnBackButtonClicked(object sender, EventArgs e)
    {
        ServiceCollectionView.IsVisible = true;
        CategoryLayout.IsVisible = false;
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue.ToLower();
        var filteredCategories = _collCategories.Where(c => c.CategoryData.Name.ToLower().Contains(searchText)).ToList();
        CategoryCollectionView.ItemsSource = filteredCategories;

        if (e.NewTextValue.Length == 0)
        {
            ServiceCollectionView.IsVisible = true;
            CategoryLayout.IsVisible = false;
            SelectedServiceLabel.Text = string.Empty;
        }
        else if (filteredCategories.Count == 0)
        {
            ServiceCollectionView.IsVisible = false;
            CategoryLayout.IsVisible = true;
            SelectedServiceLabel.Text = "No results";
        }
        else
        {
            ServiceCollectionView.IsVisible = false;
            CategoryLayout.IsVisible = true;
            SelectedServiceLabel.Text = "Matched categories";
        }
    }

    private void OnServiceSelected(object sender, TappedEventArgs e)
    {
        if (e.Parameter is ServiceDTO selService)
        {
            var filteredCategories = _collCategories.Where(c => c.CategoryData.Service_Id == selService.Id).ToList();

            SelectedServiceLabel.Text = selService.Name;
            ServiceCollectionView.IsVisible = false;
            CategoryLayout.IsVisible = true;
            CategoryCollectionView.ItemsSource = filteredCategories;
        }
    }

    private async void OnCategorySelected(object sender, TappedEventArgs e)
    {
        if (e.Parameter is CategoryViewModel selCategory)
        {
            if (!_collChosenCategories.Contains(selCategory))
            {
                _collChosenCategories.Add(selCategory);
            }

            selCategory.FrameBackgroundColor = Color.FromArgb("2D9AEA");
            await Task.Delay(400);
            selCategory.FrameBackgroundColor = Colors.Transparent;

            await this.ChoosenCategoriesScrollView.ScrollToAsync(this.ChosenCategoriesCollectionView, ScrollToPosition.End, true);
        }
    }

    private void OnChoosenCategoryDelete(object sender, TappedEventArgs e)
    {
        if (e.Parameter is CategoryViewModel delCategory)
        {
            _collChosenCategories.Remove(delCategory);
            //CategoryCollectionView.ItemsSource = _collChoosedCategories;
        }
    }

    public List<CategoryDTO> GetChosenCategories()
    {
        return _collChosenCategories.Select(catWM => catWM.CategoryData).ToList();
    }

}