using A.Interfaces;
using A.LanguageResourceManager;
using A.AppPreferences;
using A.Services;
using A.User;
using Microsoft.Maui.Platform;
using System.Globalization;
using ExceptionsHandling;


namespace A.Views;

public partial class SettingsView : ContentPage
{
    public SettingsView()
    {
        InitializeComponent();
        SetStoredSettings();
    }

    private async void LanguagePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        Picker picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        try
        {
            if (selectedIndex != -1)
            {
                CultureInfo? culture = null;

                string? choosenLanguage = (string?)picker.ItemsSource[selectedIndex];

                if (choosenLanguage != null)
                {
                    culture = SettingsService.GetCultureInfo(choosenLanguage);

                    if (culture != null)
                    {
                        App.LanguageResourceManager.SetCulture(culture);

                        if (await SettingsService.ContainsStaticAsync(PrefUserSettings.PrefLanguage))
                        {
                            Preferences.Remove(PrefUserSettings.PrefLanguage);
                        }

                        await SettingsService.SaveStaticAsync(PrefUserSettings.PrefLanguage, culture.Name);
                    }
                    else
                    {
                        throw new ExceptionHandler("UAE_001", App.UserData.CurrentCulture);
                    }
                }
                else
                {
                    throw new ExceptionHandler("UAE_002", App.UserData.CurrentCulture);
                }
            }
        }
        catch (Exception ex) when (ex is ExceptionHandler uae)
        {
            string test = uae.GetMessage;
        }
        catch (Exception ex)
        {
            ExceptionHandler uae = new ExceptionHandler(ex.Message, App.UserData.CurrentCulture, ex.InnerException);
        }
    }

    private void SetStoredSettings()
    {
        this.LanguagePicker.Title = SettingsService.GetLanguage(App.UserData.CurrentCulture);
    }
}

