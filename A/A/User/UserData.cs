using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A.User
{
    public class UserLoginInfo
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class UserSessionInfo
    {
        public string Email { get; set; }

        public string JWT { get; set; }
    }

    public class UserData
    {
        public string DefaultCulture = "";
        public string ChoosenCulture = "";

        public string DefaultUICulture = ""; // not used currently, only assigned
        public string ChoosenUICulture = ""; // not used currently, only assigned

        public string CurrentCulture => ((ChoosenCulture == string.Empty) ? DefaultCulture : ChoosenCulture);

        public UserSessionInfo UserSessionInfo = new UserSessionInfo();
        public UserLoginInfo UserLoginInfo = new UserLoginInfo();



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
