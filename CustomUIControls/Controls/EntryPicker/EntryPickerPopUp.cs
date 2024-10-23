using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;

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
    public event EventHandler<EventArgs>? OnPopUpClosedWhenItemSelected = null;
    public event EventHandler<TextChangedEventArgs>? OnEntryTextChanged = null;

    private CollectionView _collectionView;

    private Entry _searchEntry;

    private ObservableCollection<T> _displayedItems;
    private ObservableCollection<T> _backupItems = new ObservableCollection<T>();
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
                    BackgroundColor = Colors.LightBlue
                };
            }),
            SelectionMode = SelectionMode.Single,
            ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
            {
                ItemSpacing = 10 // Set the space between each item
            },
        };
        _collectionView.SelectionChanged += async (s, e) => await OnCollectionViewSelectionChanged(s, e);

        StackLayout stackLayout = new StackLayout
        {
            Children = { _searchEntry, _collectionView },
            Padding = new Thickness(20),
            BackgroundColor = Colors.White
        };

        ScrollView scrollView = new ScrollView();
        scrollView.BackgroundColor = Colors.Transparent;
        scrollView.Content = stackLayout;

        Frame frame = new Frame
        {
            CornerRadius = 20,
            Padding = 0,
            BackgroundColor = Colors.White,
            Content = scrollView
        };

        this.Content = frame;
    }

    private void OnSearcEntryTextChanged(object? sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue?.ToLower() ?? string.Empty;

        if (OnEntryTextChanged != null)
        {
            OnEntryTextChanged.Invoke(this, e);
        }
    }

    private async Task OnCollectionViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is T selectedItem)
        {
            if (OnItemSelected != null)
            {
                OnItemSelected?.Invoke(this, new ItemSelectedEventArgs<T>(selectedItem));
            }

            await CloseAsync(selectedItem);
            this._searchEntry.Unfocus();

            if (OnPopUpClosedWhenItemSelected != null)
            {
                OnPopUpClosedWhenItemSelected?.Invoke(this, new EventArgs { });
            }
        }
    }
}
