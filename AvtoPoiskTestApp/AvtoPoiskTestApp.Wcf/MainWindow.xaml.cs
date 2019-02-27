using System.Windows;
using System.Windows.Navigation;
using AvtoPoiskTestApp.Services;
using AvtoPoiskTestApp.Services.Interfaces;
using mshtml;

namespace AvtoPoiskTestApp.Wcf
{
    public partial class MainWindow : Window
    {
        private const string BaseUrl = "http://public.servicebox-parts.com/pages/index.jsp";
        private const string WorkingCatalog = "http://public.servicebox-parts.com/docprAP/";
        private const string LanguageId = "ru_RU";

        private readonly IPasswordProvider _passwordProvider;
        private readonly IEncryptionService _encryptionService;

        public MainWindow()
        {
            var c = initialCredentials.InitialCredentials;
            _passwordProvider = new PasswordProvider(c);
            _encryptionService = new EncryptionService();

            InitializeComponent();
            ProgressBar.Visibility = Visibility.Visible;
            WebBrowser.Visibility = Visibility.Hidden;
            WebBrowser.LoadCompleted += WebBrowserOnInitialLoadCompleted;
            WebBrowser.LoadCompleted += EvenLoadComplited;
            WebBrowser.Navigate(BaseUrl);
        }

        private void WebBrowserOnInitialLoadCompleted(object sender, NavigationEventArgs e)
        {
            var acc = _passwordProvider.GetNextCredentials();
            var pass = _encryptionService.Decrypt(acc.Password);
            Login(acc.Name, pass);
            WebBrowser.LoadCompleted -= WebBrowserOnInitialLoadCompleted;

        }

        private void LoginCompleted(object sender, NavigationEventArgs e)
        {

            WebBrowser.LoadCompleted -= LoginCompleted;
            SwitchLanguage();
        }

        private void Login(string login, string password)
        {
            if (!(WebBrowser.Document is HTMLDocument doc))
            {
                return;
            }

            doc.SetValueToElementById("password", password);
            doc.SetValueToElementById("username", login);
            doc.ClickElementWithId("btsubmit");
            WebBrowser.LoadCompleted += LoginCompleted;
        }

        private void SwitchLanguage()
        {
            if (!(WebBrowser.Document is HTMLDocument doc))
            {
                return;
            }

            doc.ClickElementWithId(LanguageId);
            WebBrowser.LoadCompleted += LanguageSwitched;

        }

        private void LanguageSwitched(object sender, NavigationEventArgs e)
        {
            WebBrowser.LoadCompleted -= LanguageSwitched;
            WebBrowser.Navigate(WorkingCatalog);
            WebBrowser.LoadCompleted += WorkingCatalogReached;
            GoToWorkingCatalog();
        }

        private void GoToWorkingCatalog()
        {
            
            if (!(WebBrowser.Document is HTMLDocument doc))
            {
                return;
            }

            doc.getElementById("menu").
        }

        private void WorkingCatalogReached(object sender, NavigationEventArgs e)
        {
            WebBrowser.Visibility = Visibility.Visible;
            ProgressBar.Visibility = Visibility.Hidden;
            WebBrowser.LoadCompleted -= WorkingCatalogReached;
            WebBrowser.LoadCompleted += EvenLoadComplited;
        }

        private void RemoveElements()
        {
            if (!(WebBrowser.Document is HTMLDocument doc))
            {
                return;
            }
            doc.RemoveElementWithId("tools");
            doc.RemoveElementWithId("footer");
            
        }

        private void EvenLoadComplited(object sender, NavigationEventArgs e)
        {
            RemoveElements();
        }

    }
}
