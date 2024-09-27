using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace A;

public partial class BasePage : ContentPage//, INotifyPropertyChanged
{
    //public new event PropertyChangedEventHandler? PropertyChanged;

    //public static readonly BindableProperty IsBusyProperty =
    //    BindableProperty.Create(nameof(IsBusy), typeof(bool), typeof(BasePage), false, BindingMode.TwoWay, propertyChanged: OnIsBusyChanged);

    //// This method handles when IsBusyProperty changes
    //private static void OnIsBusyChanged(BindableObject bindable, object oldValue, object newValue)
    //{
    //    var page = bindable as BasePage;
    //    if (page != null)
    //    {
    //        page.IsBusy = (bool)newValue; // Sync local IsBusy property
    //    }
    //}

    //protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    //{
    //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    base.OnPropertyChanged(propertyName);
    //}

    //private bool _isBusy;
    //public new bool IsBusy
    //{
    //    get => _isBusy;
    //    set
    //    {
    //        if (_isBusy != value)
    //        {
    //            _isBusy = value;
    //            OnPropertyChanged(nameof(IsBusy));
    //        }
    //    }
    //}

    public void SetIsBusy(bool isBusy)
    {
        LoadingIndicator.IsVisible = isBusy;
        LoadingOverlay.IsVisible = isBusy;
        base.IsBusy = isBusy;
    }

    public View ContentPlaceHolder
    {
        set => this.MainContent.Content = value;
        get => this.MainContent.Content;
    }

    public BasePage()
    {
        InitializeComponent();
        this.BindingContext = this;
    }
}