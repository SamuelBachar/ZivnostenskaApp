using A.CustomControls.ApiEndpoints;
using A.Enumerations;
using A.Extensions;
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

using Region = SharedTypesLibrary.DTOs.Bidirectional.Localization.Region;

namespace A.CustomControls.CustomPicker;

public class CustomPicker<T> : Picker
{
    public static readonly BindableProperty DataModelProperty = BindableProperty.Create(nameof(DataModel), typeof(T), typeof(CustomPicker<T>), default(T));

    public T DataModel
    {
        get => (T)GetValue(DataModelProperty);
        set => SetValue(DataModelProperty, value);
    }

    public new ObservableCollection<T> Items { get; set; } = new ObservableCollection<T>();

    private readonly HttpClient _httpClient;

    public CustomPicker(HttpClient httpClient)
    {
        _httpClient = httpClient;
        this.ItemsSource = Items;
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

        string endpoint = ApiConfig.GetEndpoint<T>(ApiAction.GetAll);

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
}
