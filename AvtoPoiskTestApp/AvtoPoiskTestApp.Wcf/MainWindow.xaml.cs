using System.Windows;
using AvtoPoiskTestApp.Services;

namespace AvtoPoiskTestApp.Wcf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var c = initialCredentials.InitialCredentials;
            var passProvider = new PasswordProvider(c);
            var acc = passProvider.GetNextCredentials();
            var pass = new EncryptionService().Decrypt(acc.Password);
            InitializeComponent();
        }
    }
}
