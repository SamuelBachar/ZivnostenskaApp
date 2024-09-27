using Microsoft.Maui;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomUIControls.Interfaces;
using CustomUIControls.Generics;
using static CustomUIControls.Enumerations.Enums;


using SharedTypesLibrary.ServiceResponseModel;
using ExtensionsLibrary.Http;

namespace CustomControlsLibrary.Controls;

public class CustomPicker<T> : Picker, IFilterable<T>
{
    public static readonly BindableProperty FilterGroupProperty = BindableProperty.Create(nameof(FilterGroup), typeof(string), typeof(CustomPicker<T>), string.Empty, propertyChanged: OnFilterGroupChanged);

    public string FilterGroup
    {
        get => (string)GetValue(FilterGroupProperty);
        set => SetValue(FilterGroupProperty, value);
    }

    public new ObservableCollection<T> Items { get; set; } = new ObservableCollection<T>();

    private HttpClient? _httpClient;
    private IEndpointResolver? _endpointResolver;
    private IRelationshipResolver? _relationshipResolver;
    private Type? _dataModel;

    public CustomPicker()
    {
        this.ItemsSource = Items;
        this.SelectedIndexChanged += OnSelectedIndexChanged;
    }

    public async Task Initialize(HttpClient httpClient, IEndpointResolver endpointResolver, IRelationshipResolver relationshipResolver, Type dataModel)
    {
        _httpClient = httpClient;
        _endpointResolver = endpointResolver;
        _relationshipResolver = relationshipResolver;
        _dataModel = dataModel;

        FilterGroupManager.Instance.Initialize(_relationshipResolver);

        await LoadData();
    }

    private async Task LoadData()
    {
        if (_httpClient == null || _endpointResolver == null || _dataModel == null)
            return;

        string endpoint = _endpointResolver.GetEndpoint<T>(ApiAction.GetAll);
        var response = await _httpClient.GetAsync(endpoint);

        response.EnsureSuccessStatusCode(); // Not HTTP Code in range <200-299>

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

    public void FilterBy(Func<T, bool> filterPredicate)
    {
        var filteredItems = Items.Where(filterPredicate).ToList();
        this.ItemsSource.Clear();

        foreach (var item in filteredItems)
        {
            ItemsSource.Add(item);
        }
    }

    private void OnSelectedIndexChanged(object? sender, EventArgs e)
    {
        var selectedItem = (T)this.SelectedItem;

        // Inform other pickers in the same FilterGroup about the change
        FilterGroupManager.Instance.NotifyPickerChanged(this, selectedItem);
    }

    // Automatically called during InitializeComponent() of View
    private static void OnFilterGroupChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CustomPicker<T> picker && newValue is string filterGroup)
        {
            if (!string.IsNullOrWhiteSpace(filterGroup))
            {
                // Automatically register the picker in the FilterGroupManager
                FilterGroupManager.Instance.RegisterPicker(picker, filterGroup);
            }
        }
    }
}
