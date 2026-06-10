using System;
using System.Windows;


namespace pr_3_pogram_mod
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //FrMain.Navigate(new Pages.TestPage());
            FrMain.Navigate(new Pages.Autho1());
        }

        private void FrMain_ContentRendered(object sender, EventArgs e)
        {
            if (FrMain.CanGoBack)
            {
                BACK.Visibility = Visibility.Visible;
            }
            else { 
                BACK.Visibility = Visibility.Hidden;
            }
        }

        private void BACK_Click(object sender, RoutedEventArgs e)
        {
            FrMain.GoBack();
        }
    }
}
