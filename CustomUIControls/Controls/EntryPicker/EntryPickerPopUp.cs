using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;

namespace CustomControlsLibrary.Controls;

public class ItemSelectedEventArgs<T> : EventArgs where T : class
{
    public T SelectedItem { get; }

    public ItemSelectedEventArgs(T selectedItem)
    {
        SelectedItem = selectedItem;
    }
}

public class EntryPickerPopUp<T> : Popup where T : class
{
    public event EventHandler<ItemSelectedEventArgs<T>>? OnItemSelected = null;
    public event EventHandler<EventArgs>? OnPopUpClosed = null;
    public event EventHandler<TextChangedEventArgs>? OnEntryTextChanged = null;

    private CollectionView _collectionView;

    private Entry _searchEntry;

    ObservableCollection<T> _displayedItems;
    public EntryPickerPopUp(ObservableCollection<T> displayedItems)
    {
        _displayedItems = displayedItems;

        _searchEntry = new Entry
        {
            Placeholder = "Search...",
            Margin = new Thickness(10)
        };
        _searchEntry.TextChanged += OnSearcEntryTextChanged;

        _collectionView = new CollectionView
        {
            ItemsSource = displayedItems,
            ItemTemplate = new DataTemplate(() =>
            {
                Label label = new Label();
                label.SetBinding(Label.TextProperty, "Name");

                return new StackLayout
                {
                    Children = { label },
                    Padding = new Thickness(10),
                    BackgroundColor = Colors.LightGray
                };
            }),
            SelectionMode = SelectionMode.Single,
            ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
            {
                ItemSpacing = 10 // Set the space between each item
            }
        };
        _collectionView.SelectionChanged += OnCollectionViewSelectionChanged;

        StackLayout stackLayout = new StackLayout
        {
            Children = { _searchEntry, _collectionView },
            Padding = new Thickness(20)
        };

        Frame frame = new Frame
        {
            CornerRadius = 20,
            Padding = 0,
            BackgroundColor = Colors.White,
            BorderColor = Colors.Blue,
            Content = stackLayout
        };

        ScrollView scrollView = new ScrollView();
        scrollView.Content = frame;

        this.Content = scrollView;
    }

    private Rect GetPopupBounds()
    {
        var displayInfo = DeviceDisplay.MainDisplayInfo;

        double screenWidth = displayInfo.Width / displayInfo.Density; // Width in pixels
        double screenHeight = displayInfo.Height / displayInfo.Density; // Height in pixels

        // Calculate the desired width and height for the popup
        double popupWidth = screenWidth * 0.8; // 80% of the screen width
        double popupHeight = screenHeight * 0.8; // 80% of the screen height

        // Return bounds for the AbsoluteLayout
        return new Rect((screenWidth - popupWidth) / 2, (screenHeight - popupHeight) / 2, popupWidth, popupHeight);
    }

    private void OnSearcEntryTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.NewTextValue))
        {
            string searchText = e.NewTextValue?.ToLower() ?? string.Empty;

            if (OnEntryTextChanged != null)
            {
                OnEntryTextChanged.Invoke(this, e);
            }
        }
        else
        {
            _collectionView.ItemsSource = _displayedItems;
        }
    }

    private void OnCollectionViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is T selectedItem)
        {
            if (OnItemSelected != null)
            {
                OnItemSelected?.Invoke(this, new ItemSelectedEventArgs<T>(selectedItem));
            }
            
            Close(selectedItem);

            if (OnPopUpClosed != null)
            {
                OnPopUpClosed?.Invoke(this, new EventArgs { });
            }
        }
    }
}
