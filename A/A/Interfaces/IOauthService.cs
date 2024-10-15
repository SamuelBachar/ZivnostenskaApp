using A.Models.OAuthTokenData;
using ExceptionsHandling;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.Models.RefreshTokenRequest;
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

    Task<(RefreshTokenResponse? refreshToken, ExceptionHandler? Exception)> RefreshAccessToken(RefreshTokenRequest request);
}
