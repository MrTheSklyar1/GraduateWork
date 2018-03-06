using ClientApp.SystemClasses;
using System.Windows;
using System.Windows.Input;

namespace ClientApp
{
    /// <summary>
    /// Логика взаимодействия для SetPassword.xaml
    /// </summary>
    public partial class SetPassword : Window
    {
        public SetPassword()
        {
            InitializeComponent();
        }

        private void PassBoxCert_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CertButton_OnClick(sender, e);
            }
        }

        private void CertButton_OnClick(object sender, RoutedEventArgs e)
        {
            SystemSingleton.CurrentSession.CertPassword = PassBoxCert.Password;
            this.Close();
        }
    }
}
