using pr_3_pogram_mod.bd;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace pr_3_pogram_mod.Pages
{
    public partial class Admin : Page
    {

        class Employee
        {

        }

        private ObservableCollection<residents> _residents;

        public Admin(users user, string role, employees employee)
        {
            InitializeComponent();
            textName.Text = $"Пользователь: {role}\n{employee.surname} {employee.name}";
            LoadResidents();
            UpdateListViewTemplate(); 
        }

        private void LoadResidents()
        {
            var residents = bdMod.GetContext(true).residents.ToList();
            _residents = new ObservableCollection<residents>(residents);
            SortResidents();
        }

        private void UpdateListViewTemplate()
        {
            if (chosenSort == null) return;

            DataTemplate template = null;

            switch (chosenSort.SelectedIndex)
            {
                case 1: // имя
                    template = CreateNameOnlyTemplate();
                    break;
                case 2: // фамилия
                    template = CreateSurnameOnlyTemplate();
                    break;
                default: // Просто (оба поля)
                    template = CreateFullTemplate();
                    break;
            }

            if (template != null)
            {
                lvRes.ItemTemplate = template;
            }
        }

        private DataTemplate CreateFullTemplate()
        {
            var template = new DataTemplate();

            var border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BorderBrushProperty, System.Windows.Media.Brushes.Black);
            border.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            border.SetValue(Border.MarginProperty, new Thickness(5));

            var grid = new FrameworkElementFactory(typeof(Grid));
            grid.SetValue(Grid.WidthProperty, 120.0);

            var rowDef1 = new FrameworkElementFactory(typeof(RowDefinition));
            rowDef1.SetValue(RowDefinition.HeightProperty, new GridLength(50));
            var rowDef2 = new FrameworkElementFactory(typeof(RowDefinition));
            rowDef2.SetValue(RowDefinition.HeightProperty, new GridLength(60));

            grid.AppendChild(rowDef1);
            grid.AppendChild(rowDef2);

            var image = new FrameworkElementFactory(typeof(Image));
            image.SetValue(Image.SourceProperty, new System.Windows.Media.Imaging.BitmapImage(
                new Uri("/Resources/iconUser.png", UriKind.Relative)));
            image.SetValue(Grid.RowProperty, 0);
            image.SetValue(Image.MarginProperty, new Thickness(5));

            var stackPanel = new FrameworkElementFactory(typeof(StackPanel));
            stackPanel.SetValue(Grid.RowProperty, 1);
            stackPanel.SetValue(StackPanel.MarginProperty, new Thickness(5));

            var nameText = new FrameworkElementFactory(typeof(TextBlock));
            var nameBinding = new System.Windows.Data.Binding("name");
            nameText.SetBinding(TextBlock.TextProperty, nameBinding);
            nameText.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            nameText.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);

            var surnameText = new FrameworkElementFactory(typeof(TextBlock));
            var surnameBinding = new System.Windows.Data.Binding("surname");
            surnameText.SetBinding(TextBlock.TextProperty, surnameBinding);
            surnameText.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);

            stackPanel.AppendChild(nameText);
            stackPanel.AppendChild(surnameText);

            grid.AppendChild(image);
            grid.AppendChild(stackPanel);
            border.AppendChild(grid);

            template.VisualTree = border;
            return template;
        }

        private DataTemplate CreateNameOnlyTemplate()
        {
            var template = new DataTemplate();

            var border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BorderBrushProperty, System.Windows.Media.Brushes.Black);
            border.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            border.SetValue(Border.MarginProperty, new Thickness(5));

            var stackPanel = new FrameworkElementFactory(typeof(StackPanel));
            stackPanel.SetValue(StackPanel.WidthProperty, 120.0);

            var image = new FrameworkElementFactory(typeof(Image));
            image.SetValue(Image.SourceProperty, new System.Windows.Media.Imaging.BitmapImage(
                new Uri("/Resources/iconUser.png", UriKind.Relative)));
            image.SetValue(Image.HeightProperty, 40.0);
            image.SetValue(Image.MarginProperty, new Thickness(5));

            var nameText = new FrameworkElementFactory(typeof(TextBlock));
            var nameBinding = new System.Windows.Data.Binding("name");
            nameText.SetBinding(TextBlock.TextProperty, nameBinding);
            nameText.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            nameText.SetValue(TextBlock.FontSizeProperty, 14.0);
            nameText.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            nameText.SetValue(TextBlock.MarginProperty, new Thickness(0, 5, 0, 10));

            stackPanel.AppendChild(image);
            stackPanel.AppendChild(nameText);
            border.AppendChild(stackPanel);

            template.VisualTree = border;
            return template;
        }

        private DataTemplate CreateSurnameOnlyTemplate()
        {
            var template = new DataTemplate();

            var border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BorderBrushProperty, System.Windows.Media.Brushes.Black);
            border.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            border.SetValue(Border.MarginProperty, new Thickness(5));

            var stackPanel = new FrameworkElementFactory(typeof(StackPanel));
            stackPanel.SetValue(StackPanel.WidthProperty, 120.0);

            var image = new FrameworkElementFactory(typeof(Image));
            image.SetValue(Image.SourceProperty, new System.Windows.Media.Imaging.BitmapImage(
                new Uri("/Resources/iconUser.png", UriKind.Relative)));
            image.SetValue(Image.HeightProperty, 40.0);
            image.SetValue(Image.MarginProperty, new Thickness(5));

            var surnameText = new FrameworkElementFactory(typeof(TextBlock));
            var surnameBinding = new System.Windows.Data.Binding("surname");
            surnameText.SetBinding(TextBlock.TextProperty, surnameBinding);
            surnameText.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            surnameText.SetValue(TextBlock.FontSizeProperty, 14.0);
            surnameText.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            surnameText.SetValue(TextBlock.MarginProperty, new Thickness(0, 5, 0, 10));

            stackPanel.AppendChild(image);
            stackPanel.AppendChild(surnameText);
            border.AppendChild(stackPanel);

            template.VisualTree = border;
            return template;
        }

        private void SortResidents()
        {
            if (_residents == null) return;

            if (chosenSort.SelectedIndex == 1)
            {
                lvRes.ItemsSource = _residents.OrderBy(r => r.name).ToList();
            }
            else if (chosenSort.SelectedIndex == 2)
            {
                lvRes.ItemsSource = _residents.OrderBy(r => r.surname).ToList();
            }
            else
            {
                lvRes.ItemsSource = _residents.ToList();
            }
        }

        private void chosenSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateListViewTemplate(); 
            SortResidents(); 
        }

        private void tbNameFind_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = tbNameFind.Text.ToLower().Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                SortResidents();
                return;
            }

            var filtered = _residents
                .Where(r => r.name.ToLower().Contains(searchText) ||
                           r.surname.ToLower().Contains(searchText))
                .ToList();

            lvRes.ItemsSource = filtered;
        }

        private void lvRes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var elem = lvRes.SelectedItem as residents;
            if (elem != null)
            {
                NavigationService.Navigate(new Pages.customeRes(elem));
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
            SortResidents();
        }

        private void brRes_Click(object sender, RoutedEventArgs e)
        {
            updateInfo();
        }

        private void btAddRes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.AddResident());
        }
    }
}