using ZivnostAPI.Models.AuthProvidersData.Facebook;
using ZivnostAPI.Models.AuthProvidersData.Google;

namespace ZivnostAPI.Models.AuthProvidersData;

public class OAuthUserInfo
{
    public GoogleUserInfo? GoogleUserInfo { get; set; }

    public FacebookUserInfo? FacebookUserInfo { get; set; }
}
