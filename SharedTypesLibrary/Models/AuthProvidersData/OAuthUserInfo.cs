using SharedTypesLibrary.Models.AuthProvidersData.Apple;
using SharedTypesLibrary.Models.AuthProvidersData.Facebook;
using SharedTypesLibrary.Models.AuthProvidersData.Google;

namespace ZivnostAPI.Models.AuthProvidersData;

public class OAuthUserInfo
{
    public GoogleUserInfo? GoogleUserInfo { get; set; }

    public FacebookUserInfo? FacebookUserInfo { get; set; }

    public AppleUserInfo? AppleUserInfo { get; set; }
}
