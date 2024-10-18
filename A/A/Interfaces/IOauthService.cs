using A.Models.OAuthTokenData;
using ExceptionsHandling;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.Models.OAuthRefreshTokenRequest;
using SharedTypesLibrary.ServiceResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A.Interfaces;

public interface IOauthService
{
    Task ReloadUserDataFromOAuthProvider(ITokenData tokenData);

    Task<(OAuthRefreshTokenResponse? refreshToken, ExceptionHandler? Exception)> RefreshAccessToken(OAuthRefreshTokenRequest request);

    Task StoreNewAccessToken(string authProvider, OAuthRefreshTokenResponse response);
}
