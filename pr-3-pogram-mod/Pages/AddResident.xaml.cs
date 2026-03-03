using pr_3_pogram_mod.bd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
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
using System.Xml.Serialization;

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

            StAddResident.IsEnabled = false;
            StAddResident.Visibility = Visibility.Hidden;

            freeApartmentKnown();
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
                int aparId = 0;
                int selectedIndex = comboBoxNumberApart.SelectedIndex;
                if (selectedIndex >= 0 && selectedIndex < freeApartment.Count)
                {
                    aparId = freeApartment[selectedIndex];
                }

                residents newRes = new residents(
                    oldId,
                    aparId,
                    name.Text,
                    surname.Text,
                    phone.Text,
                    Convert.ToInt32(res_count.Text),
                    Convert.ToDecimal(account_bal.Text)
                );


                var contextVal = new ValidationContext(newRes);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                if (!Validator.TryValidateObject(newRes, contextVal, results, true))
                {
                    string ads = "Ошибки:\n";
                    foreach (var error in results)
                    {
                        ads += $"{error.ErrorMessage}\n";
                    }
                    MessageBox.Show(ads);
                }
                else
                {
                    context.residents.Add(newRes);
                    context.SaveChanges();
                    MessageBox.Show("Резидент добавлен", "Инфо", MessageBoxButton.OK);
                }
            }

            Console.WriteLine(oldId);

            foreach (var item in userList)
            {
                idPols.Add(item.id);
                Console.WriteLine(idPols);
            }

        }

        List<int> freeApartment = new List<int>();
        List<int> freeApartmentNumber = new List<int>();

        private void freeApartmentKnown()
        {
            var allApartments = bdMod.GetContext(true).apartments.ToList();
            var allResidents = bdMod.GetContext(true).residents.ToList();

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
                    //Console.WriteLine(apartment.number);
                }
            }

            comboBoxNumberApart.ItemsSource = freeApartmentNumber;
        }

        private void btAddPol_Click_1(object sender, RoutedEventArgs e)
        {

            using (var context = new bdMod())
            {
                users newUser = new users(
                    usernameBox.Text,
                    emailBox.Text,
                    Services.Hash.HashPassword(passwordBox.Text),
                    1 + comboBoxRole.SelectedIndex,
                    true
                    );

                var contextVal = new ValidationContext(newUser);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                if (!Validator.TryValidateObject(newUser, contextVal, results, true))
                {
                    string ads = "Ошибки:\n";
                    foreach (var error in results)
                    {
                        ads += $"{error.ErrorMessage}\n";
                    }
                    MessageBox.Show(ads);
                }
                else
                {
                    Console.WriteLine($"Объект User успешно создан. Name: {newUser.username}\n");
                    context.users.Add(newUser);
                    context.SaveChanges();
                    MessageBox.Show("Пользователь добавлен", "Инфо", MessageBoxButton.OK);

                    StAddResident.IsEnabled = true;
                    StAddResident.Visibility = Visibility.Visible;

                    spAddPol.Visibility = Visibility.Hidden;
                    spAddPol.IsEnabled = false;
                }

                //newUser.email = emailBox.Text;
                //newUser.password = Services.Hash.HashPassword(passwordBox.Text);
                //newUser.role_id = 1 + comboBoxRole.SelectedIndex;
                //newUser.is_active = true;

            }

            StAddResident.IsEnabled = true;

            //Определенеи свободных комнат

        }

        private void res_count_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox.Text == "")
                textBox.Text = "0";
        }
    }
}
