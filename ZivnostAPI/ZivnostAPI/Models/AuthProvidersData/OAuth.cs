namespace ZivnostAPI.Models.AuthProvidersData;

public class OAuth
{
    public string CallBackScheme { get; set; } = string.Empty;
    public string JwtKey { get; set; } = string.Empty;

    public string RedirectUri { get; set; } = string.Empty;

    public GoogleOAuth Google { get; set; } = new GoogleOAuth();
    public FacebookOAuth Facebook { get; set; } = new FacebookOAuth();
    public AppleOAuth Apple { get; set; } = new AppleOAuth();
}

public class GoogleOAuth
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

public class FacebookOAuth
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

public class AppleOAuth
{
    public string ServiceId { get; set; } = string.Empty;
    public string KeyId { get; set; } = string.Empty;
    public string TeamId { get; set; } = string.Empty;
}
