using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using AvtoPoiskTestApp.Models;
using AvtoPoiskTestApp.Services.Interfaces;
using AvtoPoiskTestApp.Services.Unity;
using mshtml;
using Newtonsoft.Json;
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

        private IPasswordProvider _passwordProvider;
        private IEncryptionService _encryptionService;
        private IFileService _fileService;


        private void InjectServices()
        {
            ServiceInjector.ConfigureServices();
            _passwordProvider = ServiceInjector.Retrieve<IPasswordProvider>();
            _encryptionService = ServiceInjector.Retrieve<IEncryptionService>();
            _fileService = ServiceInjector.Retrieve<IFileService>();
        }

        private void InitialCreate()
        {
            var passwordsFileFullPath = _passwordProvider.GetPasswordFileName();

            if (_fileService.Exists(passwordsFileFullPath))
            {
                return;
            }

            var credentials = initialCredentials.InitialCredentials;
            var items = JsonConvert.DeserializeObject<List<Account>>(credentials);
            _fileService.SaveToFile(items, passwordsFileFullPath);
        }

        private void RegisterHandlers()
        {
            WebBrowser.LoadCompleted += WebBrowserOnLoadCompleted;
            WebBrowser.Navigating += WebBrowserOnNavigating;
            WebBrowser.PreviewKeyDown += WebBrowserOnPreviewKeyDown; //Disable hot keys
        }

        private static void WebBrowserOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                e.Handled = true;
            }

            if (e.Key == Key.F5)
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

        public MainWindow()
        {
            try
            {
                InjectServices(); //Configure unity
                InitialCreate(); //Create file with passwords if has not been created

                InitializeComponent();
                ShowLoader();

                RegisterHandlers();
                WebBrowser.Navigate(BaseUrl);
            }
            catch (Exception e)
            {
                Logger.Error(e); // Log exception
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

            DisableContextMenu(); //Disable context menu
            WebBrowser.HideJsScriptErrors();

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
    }
}
