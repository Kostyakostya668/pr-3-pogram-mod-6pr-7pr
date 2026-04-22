using pr_3_pogram_mod.bd;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

            ///Нужно чтобы был добавлен сначала пользователь
            StAddResident.IsEnabled = false;
            StAddResident.Visibility = Visibility.Hidden;

            freeApartmentKnown();
        }

        /// <summary>
        /// Добавляет нового резидента
        /// </summary>
        private void btAddRes_Click(object sender, RoutedEventArgs e)
        {
            var userList = bdMod.GetContext(true).users.ToList();
            List<int> idPols = new List<int>(); ///Для поиска юзера, чтобы связать нового пользователя с новым резидентом

            foreach (var item in userList)
            {
                idPols.Add(item.id);
            }

            int oldId = idPols.Max();

            ///Обращение к БД, для добавления резидента
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

                ///Проводится валидация данных нового резидента
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

            //Console.WriteLine(oldId);

            //foreach (var item in userList)
            //{
            //    idPols.Add(item.id);
            //    Console.WriteLine(idPols);
            //}
        }

        List<int> freeApartment = new List<int>();
        List<int> freeApartmentNumber = new List<int>();

        /// <summary>
        /// Находит свободные апартаменты, чтобы указать их у резидента
        /// </summary>
        private void freeApartmentKnown()
        {
            var allApartments = bdMod.GetContext(true).apartments.ToList();
            var allResidents = bdMod.GetContext(true).residents.ToList();

            foreach (var apartment in allApartments)
            {
                freeApartment.Add(apartment.id);
            }
            
            ///Убрать лишние id апартаментов, чтобы остались только свободные
            for (int i = 0; i < allResidents.Count; i++)
            {
                if (allResidents[i].apartment_id.HasValue)
                {
                    freeApartment.Remove(allResidents[i].apartment_id.Value);

                    Console.WriteLine(freeApartment[i]);
                }
            }

            ///Сопоставить id свободных апартаментов с их номерами
            for (int i = 0; i < freeApartment.Count; i++)
            {
                var apartment = allApartments.FirstOrDefault(a => a.id == freeApartment[i]);

                if (apartment != null && apartment.number != null)
                {
                    freeApartmentNumber.Add(Convert.ToInt32(apartment.number));
                }
            }

            comboBoxNumberApart.ItemsSource = freeApartmentNumber;
        }

        /// <summary>
        /// Добавляет нового пользователя
        /// </summary>
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

                ///Проводится валидация данных нового пользователя
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

        }

        /// <summary>
        /// Когда textBox пустой, заменяет на символ нолика  
        /// </summary>
        private void res_count_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox.Text == "")
                textBox.Text = "0";
        }
    }
}
