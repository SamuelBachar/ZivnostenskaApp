using Microsoft.Maui.Graphics.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace CustomControlsLibrary.Controls;

public partial class CustomEntry : ContentView, INotifyPropertyChanged
{
    public new event PropertyChangedEventHandler? PropertyChanged;

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public CustomEntry()
    {
        InitializeComponent();
        this.BindingContext = this;

        // Bind the Entry and Label to properties of this CustomEntry
        this.MainEntry.SetBinding(Entry.PlaceholderProperty, new Binding(nameof(Placeholder), source: this));
        this.MainEntry.SetBinding(Entry.TextColorProperty, new Binding(nameof(TextColor), source: this));
        this.MainEntry.SetBinding(Entry.KeyboardProperty, new Binding(nameof(Keyboard), source: this));
        this.MainEntry.SetBinding(Entry.TextProperty, new Binding(nameof(Text), source: this));
    }

    private bool _isFocusHandled = false;
    private DateTime _lastFocusEventTime;
    private readonly TimeSpan _debounceTime = TimeSpan.FromMilliseconds(300);

    private bool _isErrorVisible = false;
    public bool IsErrorVisible
    {
        get => _isErrorVisible;
        set
        {
            if (_isErrorVisible != value)
            {
                _isErrorVisible = value;
                OnPropertyChanged(nameof(IsErrorVisible)); // Notify UI about the change
            }
        }
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (_errorMessage != value)
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage)); // Notify UI about the change
            }
        }
    }

    // Expose Properties so they will be reachable from XAML file of Page / View
    #region Bindings of Properties
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(CustomEntry), string.Empty);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(CustomEntry), string.Empty);

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(CustomEntry), Colors.Black);

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(nameof(ErrorTextProperty), typeof(string), typeof(CustomEntry));

    public string ErrorText
    {
        get => (string)GetValue(ErrorTextProperty);
        set => SetValue(ErrorTextProperty, value);
    }

    public static readonly BindableProperty ErrorTextEmptyProperty = BindableProperty.Create(nameof(ErrorTextEmpty), typeof(string), typeof(CustomEntry));

    public string ErrorTextEmpty
    {
        get => (string)GetValue(ErrorTextEmptyProperty);
        set => SetValue(ErrorTextEmptyProperty, value);
    }

    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(CustomEntry), Keyboard.Default);
    public Keyboard Keyboard
    {
        get => (Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }
    #endregion

    // Bind the Unfocused event to the Entry
    protected override void OnParentSet()
    {
        base.OnParentSet();
        this.MainEntry.Unfocused += MainEntry_TextAdded;
    }

    public ValidationType EntryValidationType { get; set; } = ValidationType.Email;

    public bool IsMandatory { get; set; } = false;
    public enum ValidationType
    {   
        Email,
        PhoneNumber,
        Generic
    }

    public bool Validate(bool checkLength = false)
    {
        string text = this.MainEntry.Text;
        bool isValid = true;

        if (!string.IsNullOrWhiteSpace(text))
        {
            isValid = EntryValidationType switch
            {
                ValidationType.Email => ValidateEmail(text),
                ValidationType.PhoneNumber => ValidatePhoneNumber(text),
                ValidationType.Generic => true,
                _ => false
            };

            if (!isValid)
            {
                this.ErrorMessage = this.ErrorText;
                this.IsErrorVisible = true;
            }
            else
            {
                this.IsErrorVisible = false;
            }

        }

        if ((checkLength && this.IsMandatory) && string.IsNullOrWhiteSpace(text))
        {
            ErrorMessage = this.ErrorTextEmpty;
            this.IsErrorVisible = true;

            isValid = false;
        }

        return isValid;
    }

    private bool ValidateEmail(string email)
    {
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }

    private bool ValidatePhoneNumber(string phoneNumber)
    {
        string phonePattern = @"^(\+|00|0)\d{7,15}$";
        return Regex.IsMatch(phoneNumber, phonePattern);
    }

    private async void MainEntry_TextAdded(object sender, EventArgs e)
    {
        Entry? entry = sender as Entry;
        string? text = entry?.Text;
        int length = text?.Length ?? 0;

        if (length > 0)
        {
            if ((e is FocusEventArgs focArg))
            {
                _lastFocusEventTime = DateTime.Now;
                _isFocusHandled = true;

                await Task.Delay(_debounceTime);

                if (_isFocusHandled && (DateTime.Now - _lastFocusEventTime) >= _debounceTime)
                {
                    Validate();
                }

                _isFocusHandled = false;
            }
            else
            {
                Validate();
            }
        }

        if ((length == 0) && this.IsErrorVisible)
        {
            this.IsErrorVisible = false;
            this.ErrorMessage = string.Empty;
        }
    }
}