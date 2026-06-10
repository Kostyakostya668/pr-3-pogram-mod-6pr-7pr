using pr_3_pogram_mod.bd;
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

namespace pr_3_pogram_mod.Pages
{
    /// <summary>
    /// Логика взаимодействия для Client.xaml
    /// </summary>
    public partial class Client : Page
    {
        users thisUser;
        residents thisResident;

        public Client(users user, string role, residents resident)
        {
            InitializeComponent();
            if (user == null || role == null || resident == null)
            {
                textName.Text = "Вы вошли как гость";
                spUserOn.Visibility = Visibility.Hidden;
            }
            else
            {
                textName.Text = $"Пользователь: {role}\n{resident.surname} {resident.name}";
                thisUser = user; thisResident = resident;
                spUserOn.Visibility = Visibility.Visible;
            } 
        }

        private void btPrintInfo_Click(object sender, RoutedEventArgs e)
        {
            DocPage.DocPdfInfoRes docPdf = new DocPage.DocPdfInfoRes(thisUser, thisResident);
            docPdf.ShowDialog();
        }
    }

}
