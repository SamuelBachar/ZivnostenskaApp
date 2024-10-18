namespace ZivnostAPI.Models.AuthProvidersData;

public class OAuthSettings
{
    public string CallBackScheme { get; set; } = string.Empty;

    public string RedirectUri { get; set; } = string.Empty;

    public GoogleOAuth Google { get; set; } = new GoogleOAuth();
    public FacebookOAuth Facebook { get; set; } = new FacebookOAuth();
    public AppleOAuth Apple { get; set; } = new AppleOAuth();
}

public class GoogleOAuth
{
    public string BaseUrl { get; set; } = string.Empty;
    public string RetrieveAccessTokenUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

public class FacebookOAuth
{
    public string BaseUrl { get; set; } = string.Empty;
    public string RetrieveAccessTokenUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

public class AppleOAuth
{
    public string BaseUrl { get; set; } = string.Empty;
    public string RetrieveAccessTokenUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
