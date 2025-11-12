using pr_3_pogram_mod.bd;
using pr_3_pogram_mod.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace pr_3_pogram_mod.Pages
{
    /// <summary>
    /// Логика взаимодействия для Autho1.xaml
    /// </summary>
    public partial class Autho1 : Page
    {
        int click;

        private static bdMod _context;

        public static bdMod GetContext()
        {
            if (_context == null)
            {
                _context = new bdMod();
            }
            return _context;
        }

        public Autho1()
        {
            InitializeComponent();
            capthaPanel.Visibility = Visibility.Hidden;
            click = 0;
        }

        private void btnEnterGuest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Client(null, null));
        }

        private void GenerateCapctcha()
        {
            capthaPanel.Visibility = Visibility.Visible;

            string capctchaText = CaptchaGenerator.GenerateCaptchaText(6);
            txtBlockCaptha.Text = capctchaText;
            txtBlockCaptha.TextDecorations = TextDecorations.Strikethrough;
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            click += 1;
            string login = txtLogin.Text.Trim();
            string password = passwordBox.Password.Trim();
            string passwordH = Hash.HashPassword(password);

            bdMod bd = GetContext();
            
            var user = bd.users.Where(x => x.username == login && x.password == passwordH).FirstOrDefault();
            
            if (click == 1)
            {
                
                if (user != null)
                {
                    var user_role = bd.user_roles.Where(x => user.role_id == x.id).FirstOrDefault();
                    MessageBox.Show("Вы вошли под: " + user_role.role).ToString();
                    LoadPage(user_role.role, user);
                }
                else
                {
                    MessageBox.Show("Вы ввели логин или пароль неверно!");
                    GenerateCapctcha();
                    passwordBox.Clear();
                }
            }
            else if (click > 1)
            {
                if (user != null && txtBoxCaptha.Text == txtBlockCaptha.Text)
                {
                    var user_role = bd.user_roles.Where(x => user.role_id == x.id).FirstOrDefault();
                    MessageBox.Show("Вы вошли под: " + user_role.role).ToString();
                    LoadPage(user_role.role, user);
                }
                else
                {
                    MessageBox.Show("Введите данные заново!");
                }
            }
        }

        private void LoadPage(string _role, users user)
        {
            click = 0;
            switch (_role)
            {
                case "resident":
                    NavigationService.Navigate(new Client(user, _role));
                    break;
                case "admin":
                    NavigationService.Navigate(new Admin(user, _role));
                    break;
                case "employee":
                    NavigationService.Navigate(new Employee(user, _role));
                    break;
            }
        }

    }
}
