using pr_3_pogram_mod.bd;
using pr_3_pogram_mod.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace pr_3_pogram_mod.Pages
{
    /// <summary>
    /// Логика взаимодействия для Autho1.xaml
    /// </summary>
    public partial class Autho1 : Page
    {
        int click;
        DispatcherTimer timer = new DispatcherTimer();
        private int seconds = 10;

        //private static bdMod _context;

        //public static bdMod GetContext()
        //{
        //    if (_context == null)
        //    {
        //        _context = new bdMod();
        //    }
        //    return _context;
        //}

        public Autho1()
        {
            InitializeComponent();
            capthaPanel.Visibility = Visibility.Hidden;
            timer.Tick += new EventHandler(timer_Tick);
        }

        private void btnEnterGuest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Client(null, null));
            txtBoxCaptha.Clear();
            txtBlockCaptha.Text = "Капча";
            passwordBox.Clear();
            txtLogin.Clear();
            capthaPanel.Visibility = Visibility.Hidden;
            click = 0;
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

            bdMod bd = new bdMod();
            
            var user = bd.users.Where(x => x.username == login && x.password == passwordH).FirstOrDefault();
            
            if (click == 1)
            {
                if (user != null)
                {
                    var user_role = bd.user_roles.Where(x => user.role_id == x.id).FirstOrDefault();
                    MessageBox.Show("Вы вошли под: " + user_role.role).ToString();
                    LoadPage(user_role.role, user);
                    click = 0;
                }
                else
                {
                    MessageBox.Show("Вы ввели логин или пароль неверно!");
                    GenerateCapctcha();
                    passwordBox.Clear();
                }
                txtLogin.Clear();
            }
            else if (click > 1)
            {
                if (user != null && txtBoxCaptha.Text == txtBlockCaptha.Text)
                {
                    var user_role = bd.user_roles.Where(x => user.role_id == x.id).FirstOrDefault();
                    MessageBox.Show("Вы вошли под: " + user_role.role).ToString();
                    LoadPage(user_role.role, user);
                    capthaPanel.Visibility = Visibility.Hidden;
                    txtBoxCaptha.Clear();
                    click = 0;
                }
                else
                {
                    MessageBox.Show("Введите данные заново!");
                    GenerateCapctcha();
                    passwordBox.Clear();
                    txtBoxCaptha.Clear();

                }
            }

            if (click >= 3)
            {
                block(false);
                timer.Interval = new TimeSpan(0,0,1);
                timer.Start();

            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
           seconds--;
           timeBlock.Content = $"Разблокировка через: {seconds.ToString()}";
            
            if (seconds <= 0)
            {
                timer.IsEnabled = false;
                //timer.Tick -= new EventHandler(timer_Tick);
                seconds = 10;
                click = 0;
                block(true);
            }
        }

        private void block(bool blockB)
        {
            timeBlock.Content = "";
            txtLogin.IsEnabled = blockB;
            passwordBox.IsEnabled = blockB;
            capthaPanel.Visibility = Visibility.Hidden;
            ButtonPanel.IsEnabled = blockB;
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
