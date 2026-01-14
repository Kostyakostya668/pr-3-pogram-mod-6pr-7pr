using pr_3_pogram_mod.bd;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
    /// Логика взаимодействия для customeRes.xaml
    /// </summary>
    public partial class customeRes : Page
    {

        //private class res
        //{
        //    public string name { get; set; }
        //    public string surname { get; set; }
        //    public string phone { get; set; }
        //    public int idPol { get; set; }

        //    public void showInfo()
        //    {
        //        Debug.WriteLine(idPol);
        //    }
        //}

        //private res currentRes;

        private int idPol;

        public customeRes(residents resident)
        {
            InitializeComponent();

            //currentRes = new res
            //{
            //    name = resident.name,
            //    surname = resident.surname,
            //    phone = resident.phone,
            //    idPol = resident.id
            //};
            
            idPol = resident.id;
            name.Text = resident.name;
            surname.Text = resident.surname;
            phone.Text = resident.phone;
            res_count.Text = $"Количество жильцов: {resident.residents_count.ToString()}";
            account_bal.Text = $"Баланс лицевого счета: {resident.account_balance.ToString()}";
        }

        private void btSaveChan_Click(object sender, RoutedEventArgs e)
        {
            //currentRes.name = name.Text;
            //currentRes.surname = surname.Text;
            //currentRes.phone = phone.Text;

            using (var context = new bdMod())
            {
                var resident = context.residents.FirstOrDefault(r => r.id == idPol);

                if (resident != null)
                {
                    resident.name = name.Text;
                    resident.surname = surname.Text;
                    resident.phone = phone.Text;

                    context.SaveChanges();

                    MessageBox.Show("Изменения внесены");
                }
            }

        }
    }
}
