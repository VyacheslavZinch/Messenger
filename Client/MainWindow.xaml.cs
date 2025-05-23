﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///     
    

    public partial class MainWindow : Window
    {
        public static Frame mainFrame { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            mainFrame = MainFrame;
            MainFrame.Navigate(new Pages.MainWindow.MainWindowPage());
        }
    }
}