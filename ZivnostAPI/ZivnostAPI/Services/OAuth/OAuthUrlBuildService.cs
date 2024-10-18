using SharedTypesLibrary.Constants;
using ZivnostAPI.Models.AuthProvidersData;

namespace ZivnostAPI.Services.OAuth;

public class OAuthUrlBuildService
{
    private readonly OAuthSettings _oauthSettings;

    public OAuthUrlBuildService(OAuthSettings oauthSettings)
    {
        _oauthSettings = oauthSettings;
    }

    public string BuildGoogleLandingPageUrl(bool requestSignIn)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "response_type", "code" },
            { "client_id", $"{_oauthSettings.Google.ClientId}" },
            { "redirect_uri",  $"{_oauthSettings.RedirectUri}" },
            { "scope", "openid email profile" },
            { "state", $"{AuthProviders.Google}" },
            { "access_type", "offline" }
        };

        if (requestSignIn)
        {
            // Use account chooser
            queryParams.Add("prompt", "none");
            return BuildQueryString($"{_oauthSettings.Google.BaseUrl}/o/oauth2/v2/auth/oauthchooseaccount", queryParams);
        }
        else
        {
            // First-time sign-in
            queryParams.Add("prompt", "consent");
            return BuildQueryString($"{_oauthSettings.Google.BaseUrl}/v3/signin/identifier", queryParams);
        }
    }

    public string BuildFacebookLandingUrl()
    {
        var queryParams = new Dictionary<string, string>
        {
            { "response_type", "code" },
            { "client_id", $"{_oauthSettings.Facebook.ClientId}" },
            { "redirect_uri",  $"{_oauthSettings.RedirectUri}" },
            { "scope", "email,public_profile,user_mobile_phone" },
            { "state", $"{AuthProviders.Facebook}" }
        };

        return BuildQueryString($"{_oauthSettings.Facebook.BaseUrl}", queryParams);
    }

    public string BuildAppleLandingUrl()
    {
        var queryParams = new Dictionary<string, string>
        {
            { "response_type", "code" },
            { "client_id", $"{_oauthSettings.Apple.ClientId}" },
            { "redirect_uri",  $"{_oauthSettings.RedirectUri}" },
            { "scope", "email name" },
            { "state", $"{AuthProviders.Apple}" },
            { "response_mode", "form_post" }
        };

        return BuildQueryString($"{_oauthSettings.Apple.BaseUrl}", queryParams);
    }

    public Dictionary<string, string> BuildAppleRetrieveAcessTokenForm(string code)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", _oauthSettings.Apple.ClientId },
            { "client_secret", _oauthSettings.Apple.ClientSecret },
            { "redirect_uri", _oauthSettings.RedirectUri },
            { "grant_type", "authorization_code" }
        };

        return queryParams;
    }

    public string GetAppleRetrieveAcessTokenUrlEndpoint()
    {
        return _oauthSettings.Apple.RetrieveAccessTokenUrl;
    }

    public Dictionary<string, string> BuildFacebookRetrieveAccessTokenForm(string code)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", _oauthSettings.Facebook.ClientId},
            { "client_secret", _oauthSettings.Facebook.ClientSecret},
            { "redirect_uri", _oauthSettings.RedirectUri },
            { "grant_type", "authorization_code" }
        };

        return queryParams;
    }

    public string GetFacebookRetrieveAcessTokenUrlEndpoint()
    {
        return _oauthSettings.Facebook.RetrieveAccessTokenUrl;
    }

    public Dictionary<string, string> BuildGoogleRetrieveAccessTokenForm(string code)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", $"{_oauthSettings.Google.ClientId}"},
            { "client_secret", $"{_oauthSettings.Google.ClientSecret}"},
            { "redirect_uri", $"{_oauthSettings.RedirectUri}"},
            { "grant_type", "authorization_code" }
        };

        return queryParams;
    }
    public string GetGoogleRetrieveAcessTokenUrlEndpoint()
    {
        return _oauthSettings.Google.RetrieveAccessTokenUrl;
    }


    private string BuildQueryString(string baseUrl, Dictionary<string, string> queryParams)
    {
        var queryString = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
        return $"{baseUrl}?{queryString}";
    }
}
