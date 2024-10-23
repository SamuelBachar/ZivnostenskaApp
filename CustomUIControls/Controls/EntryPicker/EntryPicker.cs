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
using Microsoft.Maui.Layouts;

namespace CustomControlsLibrary.Controls;

public class EntryPicker<T> : Entry, IFilterable, IEntryPicker where T : class
{
    private Entry _entry;
    public ObservableCollection<T> _displayedItems { get; set; } = new ObservableCollection<T>();
    private bool _isPopupVisible;
    EntryPickerPopUp<T>? _popUp = null;

    public List<T> _items { get; set; } = new List<T>();
    public List<T> _itemsFilteredByParent { get; set; } = new List<T>();

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

        _items.Clear();
        _displayedItems.Clear();

        if (items is List<T> validItems)
        {
            foreach (var item in validItems)
            {
                _items.Add(item);
                _itemsFilteredByParent.Add(item);
                _displayedItems.Add(item);
            }
        }
    }

    public void FilterBy(Func<object, bool> filter, bool isFilteredByParentControl)
    {
        if (isFilteredByParentControl)
        {
            this.Text = string.Empty;
        }

        this.FilterByCustom(item => filter(item), isFilteredByParentControl);
    }

    public void FilterByCustom(Func<T, bool> filterPredicate, bool isFilteredByParentControl)
    {
        var filteredItems = _items.Where(filterPredicate).ToList();
        _displayedItems.Clear();

        if (isFilteredByParentControl)
        {
            _itemsFilteredByParent.Clear();
        }

        foreach (var item in filteredItems)
        {
            if (isFilteredByParentControl)
            {
                _itemsFilteredByParent.Add(item);
            }

            _displayedItems.Add(item);
        }
    }

    public void OnEntryTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.NewTextValue))
        {
            FilterBy(item =>
            {
                if (item is BaseDTO baseDTO)
                {
                    return baseDTO.DisplayName.Contains(e.NewTextValue, StringComparison.OrdinalIgnoreCase);
                }

                return false;
            }, isFilteredByParentControl: false);
        }
        else
        {
            RefreshList();
        }
    }

    private void OnCollectionViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is T selectedItem)
        {
            this.SelectedItem = selectedItem;

            FilterGroupManager.Instance.NotifyFilterAbleControlChange(this, selectedItem);

            if (selectedItem is BaseDTO baseDTO)
            {
                this.Text = baseDTO.DisplayName;
            }
        }
    }

    private async void OnEntryFocused(object? sender, FocusEventArgs e)
    {
        this.Unfocus();
        await ShowPopup();
    }

    private async Task ShowPopup()
    {
        _isPopupVisible = true;

        _popUp = new EntryPickerPopUp<T>(_displayedItems);
        _popUp.OnItemSelected += OnItemSelectionChanged;
        _popUp.OnPopUpClosedWhenItemSelected += OnPopUpClosedWhenItemChoosed;
        _popUp.Closed += OnPopUpClosed;
        _popUp.OnEntryTextChanged += OnEntryTextChanged;
        _popUp.Color = Colors.Transparent;

        var (screenWidth, screenHeight) = GetScreenSizes();
        var (finalWidth, finalHeight) = GetFinalLayoutSize((screenWidth, screenHeight));

        //var (screenWidth, screenHeight) = _displayService.GetDisplayDimensions();

        //double finalWidth = screenWidth * 0.8; // 80% of the screen width
        //double finalHeight = screenHeight * 0.8; // 80% of the screen height

        _popUp.Size = new Size(finalWidth, finalHeight);
        AbsoluteLayout.SetLayoutBounds(_popUp, new Rect((screenWidth - finalWidth) / 2, (screenHeight - finalHeight) / 2, finalWidth, finalHeight));
        AbsoluteLayout.SetLayoutFlags(_popUp, AbsoluteLayoutFlags.All);

        await Application.Current.MainPage?.ShowPopupAsync(_popUp);

        //var selectedItem = await Application.Current.MainPage.ShowPopupAsync(_popUp);

        _isPopupVisible = false;
    }

    private void OnPopUpClosedWhenItemChoosed(object? sender, EventArgs args)
    {
        _isPopupVisible = false;
        this.Unfocus();
        RefreshList();
    }

    private void OnPopUpClosed(object? sender, PopupClosedEventArgs args)
    {
        _isPopupVisible = false;
        this.Unfocus();
        RefreshList();
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

    public void RefreshList()
    {
        _displayedItems.Clear();

        foreach (var item in _itemsFilteredByParent)
        {
            _displayedItems.Add(item);
        }
    }

    private (double screenWidth, double screenHeight) GetScreenSizes()
    {
        var displayInfo = DeviceDisplay.MainDisplayInfo;

        double screenWidth = displayInfo.Width / displayInfo.Density; // Width in pixels
        double screenHeight = displayInfo.Height / displayInfo.Density; // Height in pixels

        return (screenWidth, screenHeight);
    }

    private (double finalScreenWidth, double finalScreenHeight) GetFinalLayoutSize((double screenWidth, double screenHeight) sizes)
    {
        return (sizes.screenWidth * 0.8, sizes.screenHeight * 0.8);
    }
}