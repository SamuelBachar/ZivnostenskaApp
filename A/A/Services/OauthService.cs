using A.Interfaces;
using A.Models.OAuthLoginData;
using A.Models.OAuthTokenData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SharedTypesLibrary.Models.AuthProvidersData.Google;
using SharedTypesLibrary.Models.AuthProvidersData.Facebook;

namespace A.Services;

class OauthService : IOauthService
{

    public OauthService() { }

    public async Task ReloadUserDataFromOAuthProvider(ITokenData tokenData)
    {
        string userInfoUrl = tokenData switch
        {
            GoogleTokenData => "https://www.googleapis.com/oauth2/v2/userinfo",
            FacebookTokenData => $"https://graph.facebook.com/me?fields=id,name,email,picture,phone",
            _ => throw new ArgumentException("Invalid provider")
        };

        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenData.AccessToken);
            HttpResponseMessage userInfoResponse = await httpClient.GetAsync(userInfoUrl);

            if (userInfoResponse.IsSuccessStatusCode)
            {
                string userInfoResString = await userInfoResponse.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(userInfoResString))
                {
                    if (tokenData is GoogleTokenData)
                    {
                        GoogleUserInfo? userInfo = JsonConvert.DeserializeObject<GoogleUserInfo>(userInfoResString);

                        if (userInfo != null)
                        {
                            App.UserData.UserIdentityData.Name = userInfo.GivenName;
                            App.UserData.UserIdentityData.SureName = userInfo.FamilyName;
                            App.UserData.UserIdentityData.Email = userInfo.Email;
                            App.UserData.UserIdentityData.PictureURL = userInfo.Picture;
                        }
                    }

                    if (tokenData is FacebookTokenData)
                    {
                        FacebookUserInfo? userInfo = JsonConvert.DeserializeObject<FacebookUserInfo>(userInfoResString);

                        if (userInfo != null)
                        {
                            App.UserData.UserIdentityData.Name = userInfo.Name;
                            App.UserData.UserIdentityData.MiddleName = userInfo.MiddleName;
                            App.UserData.UserIdentityData.SureName = userInfo.LastName;
                            App.UserData.UserIdentityData.Email = userInfo.Email;
                            App.UserData.UserIdentityData.Phone = userInfo.Phone;
                            App.UserData.UserIdentityData.PictureURL = userInfo.Picture.PictureData.Url;
                        }
                    }
                }
            }
        }
    }
}
