using CustomUIControls.Generics;
using CustomUIControls.Interfaces;
using ExtensionsLibrary.Http;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics.Text;
using SharedTypesLibrary.DTOs;
using SharedTypesLibrary.ServiceResponseModel;
using System.Collections.ObjectModel;
using static CustomUIControls.Enumerations.Enums;

namespace CustomControlsLibrary.Controls;

public partial class EntryPicker<T> : Grid, IFilterable<T>, IEntryPicker
{
    private Entry _entry;
    private CollectionView _collectionView;

    public List<T> Items { get; set; } = new List<T>();
    public ObservableCollection<T> DisplayedItems { get; set; } = new ObservableCollection<T>();

    private bool _isPopupVisible;
    private HttpClient? _httpClient;
    private IEndpointResolver? _endpointResolver;
    private IRelationshipResolver? _relationshipResolver;

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

    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(EntryPicker), default(string), BindingMode.TwoWay);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(EntryPicker), string.Empty);

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(EntryPicker), Colors.Black);

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }
    #endregion

    public EntryPicker()
    {
        InitializeControls();
        SetLayout();
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

    private void InitializeControls()
    {
        _entry = new Entry
        {
            Placeholder = this.Placeholder,
            Margin = new Thickness(10),
        };

        this._entry.SetBinding(Entry.PlaceholderProperty, new Binding(nameof(Placeholder), source: this));
        this._entry.SetBinding(Entry.TextColorProperty, new Binding(nameof(TextColor), source: this));

        _collectionView = new CollectionView
        {
            ItemsSource = DisplayedItems,
            ItemTemplate = new DataTemplate(() =>
            {
                Label label = new Label();
                label.SetBinding(Label.TextProperty, "Name");
                return new StackLayout
                {
                    Children = { label }
                };
            }),
            IsVisible = false,
            HeightRequest = 150,
        };

        _entry.TextChanged += OnEntryTextChanged;
        _entry.Focused += OnEntryFocused;
        _entry.Unfocused += OnEntryUnfocused;
        _collectionView.SelectionChanged += OnCollectionViewSelectionChanged;
    }

    private void SetLayout()
    {
        // Add Entry and CollectionView to the Grid layout
        this.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        this.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        this.Children.Add(_entry);
        this.Children.Add(_collectionView);
        Grid.SetRow(_collectionView, 1);
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

    private void OnEntryTextChanged(object? sender, TextChangedEventArgs e)
    {
        // Filter items based on the text entered
        FilterBy(item => item.ToString().Contains(e.NewTextValue, StringComparison.OrdinalIgnoreCase));

        // Show or hide CollectionView based on whether there are filtered items
        _collectionView.IsVisible = !string.IsNullOrWhiteSpace(e.NewTextValue) && DisplayedItems.Count > 0;
    }

    private void OnCollectionViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is T selectedItem)
        {
            this.SelectedItem = selectedItem;

            FilterGroupManager.Instance.NotifyFilterAbleControlChange(this, selectedItem);

            if (selectedItem is BaseDTO baseDTO)
            {
                _entry.Text = baseDTO.DisplayName;
            }
            //_entry.Text = selectedItem.ToString();
            HidePopup(); // Hide collection after selection
        }
    }

    private void OnEntryFocused(object? sender, FocusEventArgs e)
    {
        ShowPopup();
    }

    private void OnEntryUnfocused(object? sender, FocusEventArgs e)
    {
        HidePopup();
    }

    private void ShowPopup()
    {
        _isPopupVisible = true;
        _collectionView.ItemsSource = DisplayedItems;
        _collectionView.IsVisible = _isPopupVisible;
    }

    private void HidePopup()
    {
        _isPopupVisible = false;
        _collectionView.IsVisible = _isPopupVisible;
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