using pr_3_pogram_mod.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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

namespace pr_3_pogram_mod.Pages
{
    /// <summary>
    /// Логика взаимодействия для DiscoverHash.xaml
    /// </summary>
    public partial class DiscoverHash : Page
    {
        public DiscoverHash()
        {
            InitializeComponent();
        }

        private void btndiscover_Click(object sender, RoutedEventArgs e)
        {
            string password = hashText.Text;
            string passwordH = Services.Hash.HashPassword(password);
            tbPrint.Text = passwordH;
        }
    }
}
