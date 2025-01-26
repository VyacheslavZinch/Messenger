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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Pages.LoginPage
{
    /// <summary>
    /// Interaction logic for LoginFormPage.xaml
    /// </summary>
    public partial class LoginFormPage : Page
    {
        public LoginFormPage()
        {
            InitializeComponent();
        }
        private void LoginPagePasswordInput_PasswordInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void LoginPageUsernameInput_LoginInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void LoginPageSignInButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Pages.LoginPage.LoginFormPage());
        }

        private void LoginPageRestoreAccessButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoginPageCancellButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoginPageUsernameInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
