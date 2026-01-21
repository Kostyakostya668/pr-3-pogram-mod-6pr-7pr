using pr_3_pogram_mod.bd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    /// Логика взаимодействия для AddResident.xaml
    /// </summary>
    public partial class AddResident : Page
    {
        private List<int> idPol;
        //private ObservableCollection<users> _users;
        //private ObservableCollection<residents> _residents;

        //bool polHas = false;

        public AddResident()
        {
            InitializeComponent();

            //var residents = bdMod.GetContext(true).residents.ToList();
            //_residents = new ObservableCollection<residents>(residents);

            StAddResident.IsEnabled = true;
        }

        private void btAddRes_Click(object sender, RoutedEventArgs e)
        {
            //айди созданного пользователя
            var userList = bdMod.GetContext(true).users.ToList();
            List<users> ids = new List<users>();
            List<int> idPols = new List<int>();

            foreach (var item in userList)
            {
                idPols.Add(item.id);
            }

            int oldId = idPols.Max();

            using (var context = new bdMod())
            {
                residents newRes = new residents();

                newRes.user_id = oldId;
                newRes.name = name.Text;
                newRes.surname = surname.Text;
                newRes.phone = phone.Text;
                newRes.residents_count = Convert.ToInt32(res_count.Text);
                newRes.account_balance = Convert.ToDecimal(account_bal.Text);
                newRes.apartment_id = comboBoxNumberApart.SelectedIndex;

                context.residents.Add(newRes);

                context.SaveChanges();

                MessageBox.Show("Резидент добавлен", "Инфо", MessageBoxButton.OK);
            }

            Console.WriteLine(oldId);

            foreach (var item in userList)
            {
                idPols.Add(item.id);
                Console.WriteLine(idPols);
            }

        }



        private void btAddPol_Click_1(object sender, RoutedEventArgs e)
        {
            var allApartments = bdMod.GetContext(true).apartments.ToList();
            var allResidents = bdMod.GetContext(true).residents.ToList();
            List<int> freeApartment = new List<int>();
            List<int> freeApartmentNumber = new List<int>();

            foreach (var apartment in allApartments)
            {
                freeApartment.Add(apartment.id);
            }

            for (int i = 0; i < allResidents.Count; i++)
            {
                if (allResidents[i].apartment_id.HasValue)
                {
                    freeApartment.Remove(allResidents[i].apartment_id.Value);
                }
            }

            //Console.WriteLine(freeApartment.Count);

            for (int i = 0; i < freeApartment.Count; i++)
            {
                var apartment = allApartments.FirstOrDefault(a => a.id == freeApartment[i]);

                if (apartment != null && apartment.number != null)
                {
                    freeApartmentNumber.Add(Convert.ToInt32(apartment.number));
                }
            }

            comboBoxNumberApart.ItemsSource = freeApartmentNumber;


            if (usernameBox.Text == "" || emailBox.Text == "" || passwordBox.Text == "" || comboBoxRole.SelectedIndex == -1)
            {
                MessageBox.Show("Вы не ввели полную информацию", "Ошибка", MessageBoxButton.OK);

                return;
            }

            using (var context = new bdMod())
            {
                users newUser = new users();

                newUser.username = usernameBox.Text;
                newUser.email = emailBox.Text;
                newUser.password = Services.Hash.HashPassword(passwordBox.Text);
                newUser.role_id = 1 + comboBoxRole.SelectedIndex;
                newUser.is_active = true;

                context.users.Add(newUser);

                context.SaveChanges();

                MessageBox.Show("Пользователь добавлен","Инфо", MessageBoxButton.OK);
            }

            StAddResident.IsEnabled = true;

            //Определенеи свободных комнат


        }
    }
}
