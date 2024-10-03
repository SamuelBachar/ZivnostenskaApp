using ZivnostAPI.Models.AuthProvidersData.Facebook;
using ZivnostAPI.Models.AuthProvidersData.Google;

namespace ZivnostAPI.Models.AuthProvidersData;

public class OAuthUserInfo
{
    public bool IsSuccess { get; set; }
    public string Exception { get; set; } = string.Empty;

    public GoogleUserInfo? GoogleUserInfo { get; set; }

    public FacebookUserInfo? FacebookUserInfo { get; set; }
}
