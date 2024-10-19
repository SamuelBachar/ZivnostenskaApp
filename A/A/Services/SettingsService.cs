using A.AppPreferences;
using A.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static A.Enumerations.Enums;

namespace A.Services;

class SettingsService : ISettingsService
{
    public async Task<T> GetAsync<T>(string key, T defaultValue)
    {
        return await Task.Run(() =>
        {
            // var result = Preferences.Default.Get<T>(key, defaultValue);
            // return result;

            return Preferences.Default.Get<T>(key, defaultValue);
        });
    }

    public async Task SaveAsync<T>(string key, T value)
    {
        await Task.Run(() =>
        {
            Preferences.Default.Set<T>(key, value);
        });
    }

    public static async Task<T> GetStaticAsync<T>(string key, T defaultValue)
    {
        return await Task.Run(() =>
        {
            //var result = Preferences.Default.Get<T>(key, defaultValue);
            //return result;

            return Preferences.Default.Get<T>(key, defaultValue);
        });
    }

    public static async Task SaveStaticAsync<T>(string key, T value)
    {
        await Task.Run(() =>
        {
            Preferences.Default.Set<T>(key, value);
        });
    }

    public static async Task RemoveStaticAsync(string key)
    {
        await Task.Run(() =>
        {
            Preferences.Default.Remove(key);
        });
    }

    public static async Task<bool> ContainsStaticAsync(string key)
    {
        //bool result = false;
        
        return await Task.Run(() =>
        {
            //bool result = Preferences.Default.ContainsKey(key);
            //return result;

            return Preferences.Default.ContainsKey(key);
        });

        //return result;
    }

    public static CultureInfo? GetCultureInfo(string choosenLanguage)
    {
        CultureInfo? cultureInfo = null;

        if ((choosenLanguage == "Slovakia") || (choosenLanguage == "sk"))
            cultureInfo = new CultureInfo("sk");

        if (choosenLanguage == "Czech" || (choosenLanguage == "cs"))
            cultureInfo = new CultureInfo("cs");

        if (choosenLanguage == "Germany" || (choosenLanguage == "de"))
            cultureInfo = new CultureInfo("de");

        if (choosenLanguage == "English" || (choosenLanguage == "en"))
            cultureInfo = new CultureInfo("en");

        if (choosenLanguage == "TestExc" || (choosenLanguage == "testExc"))
            cultureInfo = new CultureInfo("testExc");

        return cultureInfo;
    }

    public static string GetLanguage(string choosenCulture)
    {
        string language = string.Empty;

        if (choosenCulture == "sk" || choosenCulture == "sk-SK")
            language = "Slovakia";

        if  (choosenCulture == "cs")
            language = "Czech";

        if (choosenCulture == "de")
            language = "Germany";

        if (choosenCulture == "en")
            language = "English";

        if (choosenCulture == "testExc")
            language = "testExc";

        return language;
    }

    public static async Task SavePreferedApplicationMode(AppMode appMode, bool saveAppMode)
    {
        if (saveAppMode)
        {
            await SettingsService.SaveStaticAsync<AppMode>($"{PrefUserSettings.PrefAppModeChoice}_{App.UserData.UserIdentityData.Id}", appMode);
        }
    }

    // Check if User choosed Application Mode and (Marked / Checked) to remember Application Mode in LogInChooseView
    public static async Task<bool> IsPrefRememberAppModeChoiceStored()
    {
        return await SettingsService.ContainsStaticAsync($"{PrefUserSettings.PrefRememberAppModeChoice}_{App.UserData.UserIdentityData.Id}");
    }

    public static async Task<AppMode> GetPreferedApplicationMode(AppMode appModeIfFails)
    {
       return await SettingsService.GetStaticAsync<AppMode>($"{PrefUserSettings.PrefAppModeChoice}_{App.UserData.UserIdentityData.Id}", appModeIfFails);
    }
}
