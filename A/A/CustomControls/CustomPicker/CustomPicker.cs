using A.CustomControls.UIControlsApiEndpoints;
using A.Enumerations;
using A.Extensions;
using A.Interfaces;
using Microsoft.Maui;
using Newtonsoft.Json;
using SharedTypesLibrary.DTOs.Bidirectional.Localization;
using SharedTypesLibrary.ServiceResponseModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A.CustomControls.FilterGroupManager;

using Region = SharedTypesLibrary.DTOs.Bidirectional.Localization.Region;

namespace A.CustomControls.CustomPicker;

public class CustomPicker<T> : Picker, IFilterable<T>
{
    public static readonly BindableProperty DataModelProperty = BindableProperty.Create(nameof(DataModel), typeof(T), typeof(CustomPicker<T>), default(T));

    public T DataModel
    {
        get => (T)GetValue(DataModelProperty);
        set => SetValue(DataModelProperty, value);
    }

    public new ObservableCollection<T> Items { get; set; } = new ObservableCollection<T>();

    private readonly HttpClient _httpClient;

    public string FilterGroup { get; set; }
    private List<T> _originalItems;

    public CustomPicker(HttpClient httpClient)
    {
        _httpClient = httpClient;
        this.ItemsSource = Items;

        this.SelectedIndexChanged += OnSelectedIndexChanged;
    }

    protected override async void OnParentSet()
    {
        base.OnParentSet();
        await LoadData();
    }

    private async Task LoadData()
    {
        if (DataModel == null)
            return;

        string endpoint = UIEndPoints.GetEndpoint<T>(ApiAction.GetAll);

        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode(); // todo throw an exception
            ApiResponse<T> apiResponse = await response.Content.ExtReadFromJsonAsync<T>();
            var items = apiResponse.ListData;

            Items.Clear();

            if (items is List<T> validItems)
            {
                foreach (var item in validItems)
                {
                    Items.Add(item);
                }
            }
        }
        catch (Exception ex)
        {
            // Handle error (e.g., log or notify user)  --> forward somehow smart
        }
    }

    public void FilterBy(Func<T, bool> filterPredicate)
    {
        var filteredItems = Items.Where(filterPredicate).ToList();
        this.ItemsSource.Clear();

        foreach (var item in filteredItems)
        {
            ItemsSource.Add(item);
        }
    }

    private void OnSelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedItem = (T)this.SelectedItem;
        // Inform other pickers in the same FilterGroup about the change
        FilterGroupManager.FilterGroupManager.Instance.NotifyPickerChanged(this, selectedItem);
    }
}
