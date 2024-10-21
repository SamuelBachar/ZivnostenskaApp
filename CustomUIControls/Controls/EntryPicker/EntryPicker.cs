using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CustomUIControls.Generics;
using CustomUIControls.Interfaces;
using ExtensionsLibrary.Http;
using SharedTypesLibrary.DTOs;
using SharedTypesLibrary.ServiceResponseModel;
using System.Collections.ObjectModel;
using Microsoft.Maui.Devices;

using static CustomUIControls.Enumerations.Enums;
using CustomControlsLibrary.Interfaces;

namespace CustomControlsLibrary.Controls;

public class EntryPicker<T> : Entry, IFilterable, IEntryPicker where T : class
{
    private Entry _entry;
    public ObservableCollection<T> DisplayedItems { get; set; } = new ObservableCollection<T>();
    private bool _isPopupVisible;
    EntryPickerPopUp<T>? _popUp = null;

    public List<T> Items { get; set; } = new List<T>();

    private HttpClient? _httpClient;
    private IEndpointResolver? _endpointResolver;
    private IRelationshipResolver? _relationshipResolver;
    private IDisplayService _displayService;

    private Type? _dataModel;
    public Type DataModelType
    {
        get => _dataModel;
    }

    // Expose Properties so they will be reachable from XAML file of Page / View
    #region Bindings of Properties

    public static readonly BindableProperty FilterGroupProperty = BindableProperty.Create(nameof(FilterGroup), typeof(string), typeof(EntryPicker<T>), string.Empty, propertyChanged: OnFilterGroupChanged);

    public string FilterGroup
    {
        get => (string)GetValue(FilterGroupProperty);
        set => SetValue(FilterGroupProperty, value);
    }

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(T), typeof(EntryPicker<T>), default(T?), BindingMode.TwoWay);

    public T SelectedItem
    {
        get => (T)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    //public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(EntryPicker<T>), default(string), BindingMode.TwoWay);

    //public string Text
    //{
    //    get => (string)GetValue(TextProperty);
    //    set => SetValue(TextProperty, value);
    //}

    //public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(EntryPicker<T>), string.Empty);

    //public string Placeholder
    //{
    //    get => (string)GetValue(PlaceholderProperty);
    //    set => SetValue(PlaceholderProperty, value);
    //}

    //public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(EntryPicker<T>), Colors.Black);

    //public Color TextColor
    //{
    //    get => (Color)GetValue(TextColorProperty);
    //    set => SetValue(TextColorProperty, value);
    //}
    #endregion

    public EntryPicker()
    {
        this.Focused += OnEntryFocused;
    }

    public async Task Initialize(HttpClient httpClient, IEndpointResolver endpointResolver, IRelationshipResolver relationshipResolver, IDisplayService displayService, Type dataModel)
    {
        _httpClient = httpClient;
        _endpointResolver = endpointResolver;
        _relationshipResolver = relationshipResolver;
        _displayService = displayService;
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

        string test = await response.Content.ReadAsStringAsync();
        ApiResponse<T> apiResponse = await response.Content.ExtReadFromJsonAsync<T>();
        var items = apiResponse.ListData;

        Items.Clear();
        DisplayedItems.Clear();

        if (items is List<T> validItems)
        {
            foreach (var item in validItems)
            {
                Items.Add(item);
                DisplayedItems.Add(item);
            }
        }
    }

    public void FilterBy(Func<object, bool> filter)
    {
        this.FilterByCustom(item => filter(item));
    }

    public void FilterByCustom(Func<T, bool> filterPredicate)
    {
        var filteredItems = Items.Where(filterPredicate).ToList();
        DisplayedItems.Clear();

        foreach (var item in filteredItems)
        {
            DisplayedItems.Add(item);
        }
    }

    public void OnEntryTextChanged(object? sender, TextChangedEventArgs e)
    {
        // Filter items based on the text entered
        FilterBy(item => 
        {
            if (item is BaseDTO baseDTO)
            {
                 return baseDTO.DisplayName.Contains(e.NewTextValue, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        });
    }

    private async void OnCollectionViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is T selectedItem)
        {
            this.SelectedItem = selectedItem;

            FilterGroupManager.Instance.NotifyFilterAbleControlChange(this, selectedItem);

            if (selectedItem is BaseDTO baseDTO)
            {
               this.Text = baseDTO.DisplayName;
            }
            
            //await HidePopup(); // Hide collection after selection
        }
    }

    private async void OnEntryFocused(object? sender, FocusEventArgs e)
    {
        await ShowPopup();
    }

    private async Task HidePopup()
    {
        if (_popUp != null && _isPopupVisible)
        {
            await _popUp.CloseAsync();
            _isPopupVisible = false;
        }
    }

    private async Task ShowPopup()
    {
        _isPopupVisible = true;

        _popUp = new EntryPickerPopUp<T>(DisplayedItems);
        _popUp.OnItemSelected += OnItemSelectionChanged;
        _popUp.OnPopUpClosed += OnPopUpClosed;
        _popUp.OnEntryTextChanged += OnEntryTextChanged;

        var (screenWidth, screenHeight) = _displayService.GetDisplayDimensions();

        double popupWidth = screenWidth * 0.8; // 80% of the screen width
        double popupHeight = screenHeight * 0.8; // 80% of the screen height

        _popUp.Size = new Size(popupWidth, popupHeight);

        AbsoluteLayout.SetLayoutBounds(_popUp, new Rect((screenWidth - popupWidth) / 2, (screenHeight - popupHeight) / 2, popupWidth, popupHeight));

        await Application.Current.MainPage?.ShowPopupAsync(_popUp);

        //var selectedItem = await Application.Current.MainPage.ShowPopupAsync(_popUp);

        _isPopupVisible = false;
    }

    private void OnPopUpClosed(object? sender, EventArgs args)
    {
        _isPopupVisible = false;
        this.Unfocus();
    }

    private void OnItemSelectionChanged(object? sender, ItemSelectedEventArgs<T> e)
    {
        this.SelectedItem = e.SelectedItem;

        if (e.SelectedItem is BaseDTO baseDTO)
        {
            this.SelectedItem = e.SelectedItem;
            this.Text = baseDTO.DisplayName;

            FilterGroupManager.Instance.NotifyFilterAbleControlChange(this, this.SelectedItem);
        }
    }

    // Automatically called during InitializeComponent() of View
    private static void OnFilterGroupChanged(BindableObject bindable, object oldValue, object newValue)
    {
       if (bindable is EntryPicker<T> entryPicker && newValue is string filterGroup)
        {
            if (!string.IsNullOrWhiteSpace(filterGroup))
            {
                // Automatically register the picker in the FilterGroupManager
                FilterGroupManager.Instance.RegisterFilterAbleControl(entryPicker, filterGroup);
            }
        }
    }
}