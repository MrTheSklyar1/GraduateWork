using ClientApp.SystemClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
