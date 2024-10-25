using SharedTypesLibrary.DTOs.Bidirectional.Categories;
using SharedTypesLibrary.DTOs.Bidirectional.Services;
using System.Collections.ObjectModel;

namespace A.CustomControls.Controls;

public partial class ServiceCategoryList : ContentView
{
    private ObservableCollection<ServiceDTO> Services = new ObservableCollection<ServiceDTO>();
    private ObservableCollection<CategoryDTO> Categories = new ObservableCollection<CategoryDTO>();
    public ServiceCategoryList()
	{
		InitializeComponent();
        LoadServices();
    }

    private void LoadServices()
    {
        // Load services with name and image (mock data for example)
        Services.Add(new ServiceDTO { Name = "Architecture", Image = "architecture.png" });
        Services.Add(new ServiceDTO { Name = "Plumbing", Image = "plumbing.png" });
        // Add more services...

        ServiceCollectionView.ItemsSource = Services;
    }

    private void OnServiceSelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedService = (ServiceDTO)e.CurrentSelection.FirstOrDefault();
        if (selectedService != null)
        {
            // Load corresponding categories (mock data for example)
            Categories.Clear();
            Categories.Add(new CategoryDTO { Name = "Architectural Design", Service_Id = 1 });
            Categories.Add(new CategoryDTO { Name = "Interior Design", Service_Id = 1 });
            // Add more categories...

            // Display categories
            SelectedServiceLabel.Text = selectedService.Name;
            ServiceCollectionView.IsVisible = false;
            CategoryLayout.IsVisible = true;
            CategoryCollectionView.ItemsSource = Categories;
        }
    }

    private void OnBackButtonClicked(object sender, EventArgs e)
    {
        // Go back to service list
        ServiceCollectionView.IsVisible = true;
        CategoryLayout.IsVisible = false;
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        // Filter categories based on search query
        var searchText = e.NewTextValue.ToLower();
        var filteredCategories = Categories.Where(c => c.Name.ToLower().Contains(searchText)).ToList();
        CategoryCollectionView.ItemsSource = filteredCategories;
    }
}