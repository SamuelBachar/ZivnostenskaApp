using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A.User
{

    public class UserAuthenticationData
    {
        public string JWT { get; set; } = string.Empty;

        public string JWTRefreshToken { get; set; } = string.Empty;

        public bool IsOAuthLogin { get; set; }

        public string Provider { get; set; } = string.Empty;

        public string OAuthAccessToken { get; set; } = string.Empty;

        public string OAuthRefreshToken { get; set; } = string.Empty;

        public int OAuthExpiresIn { get; set; }
    }

    public class UserIdentityData
    {
        public int Id { get; set; }

        public string OAuthId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string PictureURL { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string SureName { get; set; } = string.Empty;

        public bool NewUser { get; set; }
        public bool RegisteredAsCustomer { get; set; }
        public bool RegisteredAsCompany { get; set; }
    }

    public class UserData
    {
        public string DefaultCulture = "";
        public string ChoosenCulture = "";

        public string DefaultUICulture = ""; // not used currently, only assigned
        public string ChoosenUICulture = ""; // not used currently, only assigned

        public string CurrentCulture => ((ChoosenCulture == string.Empty) ? DefaultCulture : ChoosenCulture);

        public UserAuthenticationData UserAuthData = new UserAuthenticationData();

        public UserIdentityData UserIdentityData = new UserIdentityData();

        static UserData? _instance = null;

        private UserData() { }

        public static UserData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new();
                }

                return _instance;
            }
        }

    }
}
