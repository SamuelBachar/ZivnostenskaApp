using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Google.Crypto.Tink.Signature;

namespace A.User
{
    public class UserData
    {
        public string DefaultCulture = "";
        public string ChoosenCulture = "";

        public string DefaultUICulture = ""; // not used currently, only assigned
        public string ChoosenUICulture = ""; // not used currently, only assigned

        public string CurrentCulture => ((ChoosenCulture == string.Empty) ? DefaultCulture : ChoosenCulture);
        public string JWT = "";

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
