using Android.App;
using Android.Content.PM;
using Microsoft.Maui.Authentication;

[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter(new[] { Android.Content.Intent.ActionView },
              Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable },
              DataScheme = "https://18745623496987751/majster_app/auth")]
              //DataHost = "18745623496987751",
              //DataPath = "/majster_app/auth")]
public class WebAuthenticationCallbackActivity: WebAuthenticatorCallbackActivity
{
}
