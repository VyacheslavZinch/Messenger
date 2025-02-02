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

namespace Client.Pages.RegistrationPage
{
    /// <summary>
    /// Interaction logic for RegistrationFormPage.xaml
    /// </summary>
    public partial class RegistrationFormPage : Page
    {
        public RegistrationFormPage()
        {
            InitializeComponent();
        }
        private void RegistrationPageConfirmRegistration_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.RegistrationPage.RegistrationCompletePage());

        }

        private void RegistrationPageCancelRegistration_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.MainWindow.MainWindowPage());

        }

        private void RegistrationPageNameInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void RegistrationPageNicknameInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void RegistrationPageMailInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

