using A.Interfaces;
using A.LanguageResourceManager;
using A.AppPreferences;
using A.Services;
using A.User;
using Microsoft.Maui.Platform;
using System.Globalization;
using A.Exceptions.UserActionException;


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
                        throw new UserActException("UAE_001");
                    }
                }
                else
                {
                    throw new UserActException("UAE_002");
                }
            }
        }
        catch (Exception ex) when (ex is UserActException uae)
        {
            string test = uae.GetMessage;
        }
        catch (Exception ex)
        {
            UserActException uae = new UserActException(ex.Message, ex.InnerException);

        }
    }

    private void SetStoredSettings()
    {
        this.LanguagePicker.Title = SettingsService.GetLanguage(App.UserData.CurrentCulture);
    }
}

