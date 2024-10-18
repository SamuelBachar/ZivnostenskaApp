#pragma warning disable CS8618

using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A.ViewModels;


[QueryProperty("OAuthRegistration", "OAuthRegistration")]
[QueryProperty("Provider", "Provider")]
[QueryProperty("IsPreferredAppModeChecked", "IsPreferredAppModeChecked")]

public partial class RegisterCompanyViewModel : ObservableObject
{
    [ObservableProperty]
    public bool oAuthRegistration;

    [ObservableProperty]
    public string provider;

    [ObservableProperty]
    public bool isPreferredAppModeChecked;
}

#pragma warning restore CS8618
