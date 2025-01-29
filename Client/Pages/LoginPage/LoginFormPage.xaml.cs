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
    /// 
    public partial class LoginFormPage : Page
    {
        private string Login{ get; set; } = string.Empty;
        private string Password{ get; set; } = String.Empty;
        public LoginFormPage()
        {
            InitializeComponent();
        }

        private void LoginPageSignInButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"{Login} {Password}");
            NavigationService?.Navigate(new Pages.RegistrationPage.RegistrationCompletePage());
        }

        private void LoginPageRestoreAccessButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.RestoreAccess.RestoreAccesFormPage());

        }

        private void LoginPageCancellButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.MainWindow.MainWindowPage());

        }

        private void LoginPageUsernameInput_LoginInput(object sender, TextChangedEventArgs e)
        {
            Login = LoginPageUsernameInput.Text;
        }

        private void LoginPagePasswordInput_PasswordInput(object sender, RoutedEventArgs e)
        {
            Password = LoginPagePasswordInput.Password;
        }
    }
}
