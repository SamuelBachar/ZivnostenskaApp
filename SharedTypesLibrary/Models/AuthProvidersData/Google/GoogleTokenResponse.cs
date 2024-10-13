using Newtonsoft.Json;

namespace ZivnostAPI.Models.AuthProvidersData.Google;

public class GoogleTokenResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonProperty("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;

    [JsonProperty("id_token")]
    public string IdToken { get; set; } = string.Empty;
}

