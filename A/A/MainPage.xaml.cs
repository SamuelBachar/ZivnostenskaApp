using A.LanguageResourceManager;
using A.Resources.LanguageResources;
using System.Globalization;

namespace A
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public LanguageManager LanguageResourceManager => LanguageManager.Instance;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this; // not needed anymore or ?
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            var switchToCulture = LanguageResource.Culture.TwoLetterISOLanguageName.Equals("sk", StringComparison.InvariantCultureIgnoreCase) ? 
                                  new CultureInfo("en-US") 
                                  : 
                                  new CultureInfo("sk");

            App.LanguageResourceManager.SetCulture(switchToCulture);

            count++;
            this.TestButton.Text = String.Format(LanguageResourceManager["Counter"].ToString(), count);

            SemanticScreenReader.Announce(this.TestButton.Text);
        }
    }
}
