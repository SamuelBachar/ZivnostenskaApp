using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using A.Resources.LanguageResources;

namespace A.LanguageResourceManager
{
    public class LanguageManager : INotifyPropertyChanged
    {
        static LanguageManager? _instance = null;
        public event PropertyChangedEventHandler? PropertyChanged = null;

        private LanguageManager()
        {
            LanguageResource.Culture = CultureInfo.CurrentCulture;
        }

        public static LanguageManager Instance { 
            get
            {
                if (_instance == null)
                {
                    _instance = new ();
                }

                return _instance;
            }
        }

        public object this[string resourceKey] => LanguageResource.ResourceManager.GetObject(resourceKey, LanguageResource.Culture) ?? Array.Empty<byte>();

        public void SetCulture(CultureInfo culture)
        {
            LanguageResource.Culture = culture;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
