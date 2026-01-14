using pr_3_pogram_mod.bd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    /// Логика взаимодействия для Admin.xaml
    /// </summary>
    public partial class Admin : Page
    {

        private ObservableCollection<residents> _residents;


        public Admin(users user, string role, employees employee)
        {
            InitializeComponent();
            textName.Text = $"Пользователь: {role}\n{employee.surname} {employee.name}";
            LoadResidents();

        }

        private void LoadResidents()
        {
            var residents = bdMod.GetContext(true).residents.ToList();
            _residents = new ObservableCollection<residents>(residents);
            lvRes.ItemsSource = _residents;
        }

        private void tbNameFind_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = tbNameFind.Text;

            var resList = bdMod.GetContext(true).residents.ToList();
            List<residents> names = new List<residents>();


            foreach (var item in resList)
            {
                if (name.ToLower().Trim() == item.name.ToLower())
                {
                    names.Add(item);
                }
            }

            if (names.Any())
                lvRes.ItemsSource = names;
            else
                lvRes.ItemsSource = resList;
        }

        private void lvRes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var elem = lvRes.SelectedItem as residents;

            if (elem != null)
            {
                NavigationService.Navigate(new Pages.customeRes(elem));
            }
            else
            {
                elem = null;
            }
        }

        private void updateInfo()
        {
            var freshData = bdMod.GetContext(true).residents.ToList();

            _residents.Clear();
            foreach (var item in freshData)
            {
                _residents.Add(item);
            }
        }

        private void brRes_Click(object sender, RoutedEventArgs e)
        {
            updateInfo();
        }
    }
}
