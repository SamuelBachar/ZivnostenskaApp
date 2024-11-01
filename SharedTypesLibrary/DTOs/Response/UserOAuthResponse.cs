﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.DTOs.Response;

public class UserOAuthResponse
{
    public int Id { get; set; }
    public string OAuthId { get; set; } = string.Empty;

    //public string Provider { get; set; } = string.Empty;

    /* User profile data */
    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string PictureURL { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string SureName { get; set; } = string.Empty;
    /* ***************** */

    /* JWT Used for all clients regardless of authentication method */
    public string JWT { get; set; } = string.Empty;
    public string JWTRefreshToken { get; set; } = string.Empty;
    /* ************************************************************ */

    /* Used against further login via OAuth providers if not expired yet */
    public string OAuthAccessToken { get; set; } = string.Empty;

    public string OAuthRefreshToken { get; set; } = string.Empty;

    public int OAuthExpiresIn { get; set; }

    public string OAuthAppleJwtToken { get; set; } = string.Empty;
    /* ****************************************************************** */

    public bool NewUser { get; set; }

    public bool RegisteredAsCustomer { get;set; }

    public bool RegisteredAsCompany { get;set; }
}
