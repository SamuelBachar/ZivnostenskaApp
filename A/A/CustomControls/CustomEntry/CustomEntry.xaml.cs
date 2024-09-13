using Microsoft.Maui.Graphics.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace A.CustomControls.CustomEntry;

public partial class CustomEntry : ContentView
{
    public CustomEntry()
    {
        InitializeComponent();

        // Bind the Entry and Label to properties of this CustomEntry
        this.MainEntry.SetBinding(Entry.PlaceholderProperty, new Binding(nameof(Placeholder), source: this));
        this.MainEntry.SetBinding(Entry.TextColorProperty, new Binding(nameof(TextColor), source: this));
        this.MainEntry.SetBinding(Entry.KeyboardProperty, new Binding(nameof(Keyboard), source: this));
    }

    // Expose Properties so they will be reachable from XAML file of Page / View
    #region Bindings of Properties
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(CustomEntry), string.Empty, propertyChanged: OnTextChanged);

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
        PhoneNumber
    }

    public bool Validate(bool checkLength = false)
    {
        string text = this.MainEntry.Text;
        bool isValid = true;

        if (!checkLength)
        {
            isValid = EntryValidationType switch
            {
                ValidationType.Email => ValidateEmail(text),
                ValidationType.PhoneNumber => ValidatePhoneNumber(text),
                _ => false
            };

            if (!isValid)
            {
                LblError.Text = this.EntryValidationType == ValidationType.Email ?
                    "Bad E-mail" :
                    "Bad Phone number";

                LblError.IsVisible = true;
            }
            else
            {
                LblError.IsVisible = false;
            }
        }
        else
        {
            if (this.IsMandatory)
            {
                isValid = string.IsNullOrWhiteSpace(text);
                LblError.Text = "Cannot be empty";
            }
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

    private void MainEntry_TextAdded(object sender, EventArgs e)
    {
        if (((Entry)(sender)).Text?.Length != 0)
        {
            if ((e is FocusEventArgs focArg))
            {
                Validate();
            }

            if ((e is EventArgs comArg))
            {
                Validate();
            }
        }
    }

    // Needed due to Compilation
    private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomEntry)bindable;
    }
}