using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using AvtoPoiskTestApp.Services;
using AvtoPoiskTestApp.Services.Interfaces;
using mshtml;
using NLog;

namespace AvtoPoiskTestApp.Wcf
{
    public partial class MainWindow : Window
    {
        private const string BaseUrl = "http://public.servicebox-parts.com/pages/index.jsp";
        private const string WorkingCatalog = "http://public.servicebox-parts.com/docprAP/";
        private const string LanguageId = "ru_RU";
        private bool _initialLoadPassed;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); //Set up NLog

        private readonly IPasswordProvider _passwordProvider;
        private readonly IEncryptionService _encryptionService;

        public MainWindow()
        {
            try
            {
                var credentials = initialCredentials.InitialCredentials;
                _passwordProvider = new PasswordProvider(credentials);
                _encryptionService = new EncryptionService();

                InitializeComponent();
                ShowLoader();

                WebBrowser.LoadCompleted += WebBrowserOnLoadCompleted;
                WebBrowser.Navigating += WebBrowserOnNavigating;
                WebBrowser.Navigate(BaseUrl);

                WebBrowser.PreviewKeyDown += WebBrowserOnPreviewKeyDown; //Disable backspace
            }
            catch (Exception e)
            {
                Logger.Error(e); // Log exception
            }
        }

        private static void WebBrowserOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                e.Handled = true;
            }
        }

        private void WebBrowserOnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (_initialLoadPassed)
            {
                ShowLoader();
            }
        }

        private void ShowLoader()
        {
            ProgressBar.Visibility = Visibility.Visible;
            WebBrowser.Visibility = Visibility.Hidden;
        }

        private void HideLoader()
        {
            ProgressBar.Visibility = Visibility.Hidden;
            WebBrowser.Visibility = Visibility.Visible;
        }

        private void DisableContextMenu()
        {
            if (!(WebBrowser.Document is HTMLDocumentEvents2_Event docEventListener))
            {
                return;
            }

            docEventListener.oncontextmenu += obj => false;
        }

        private void WebBrowserOnLoadCompleted(object sender, NavigationEventArgs e)
        {

            if (!(WebBrowser.Document is HTMLDocument doc))
            {
                return;
            }

            DisableContextMenu();
            HideJsScriptErrors(WebBrowser);

            if (doc.IsLoginPage())
            {
                Login();
            }
            else
            {
                RemoveElements();
                if (!doc.IsRusLanguage())
                {
                    SwitchLanguage();
                }

                if (!doc.IsInPage(WorkingCatalog))
                {
                    if (!_initialLoadPassed)
                    {
                        WebBrowser.Navigate(WorkingCatalog);
                    }
                }
                else
                {
                    _initialLoadPassed = true;
                    HideLoader();
                }

                if (_initialLoadPassed)
                {
                    HideLoader();
                }
            }
        }

        private void Login()
        {
            var account = _passwordProvider.GetNextCredentials();
            var password = _encryptionService.Decrypt(account.Password);
            var login = account.Name;

            if (!(WebBrowser.Document is HTMLDocument doc))
            {
                return;
            }

            doc.SetValueToElementById("password", password);
            doc.SetValueToElementById("username", login);
            doc.ClickElementWithId("btsubmit");
        }

        private void SwitchLanguage()
        {
            if (!(WebBrowser.Document is HTMLDocument doc))
            {
                return;
            }

            doc.ClickElementWithId(LanguageId);
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

        private static void HideJsScriptErrors(WebBrowser wb)
        {
            var fld = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fld == null)
                return;
            var obj = fld.GetValue(wb);
            obj?.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, obj, new object[] {true});
        }
    }
}
