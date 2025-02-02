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

namespace Client.Pages.RestoreAccess
{
    /// <summary>
    /// Interaction logic for RestoreAccesFormPage.xaml
    /// </summary>
    public partial class RestoreAccesFormPage : Page
    {
        private string usersEmail { get; set; } = String.Empty;
        public RestoreAccesFormPage()
        {
            InitializeComponent();
        }

        private void RestoreAccess_MailInput(object sender, TextCompositionEventArgs e)
        {
            usersEmail = e.Text;
        }

        private void RestoreAccessCancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.MainWindow.MainWindowPage());
        }

        private void RestoreAccessConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.RestoreAccess.RestoreAccessCompletePage());
        }
    }
}
