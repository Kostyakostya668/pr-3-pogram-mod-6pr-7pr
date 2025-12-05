using pr_3_pogram_mod.bd;
using pr_3_pogram_mod.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Xml.Linq;

namespace pr_3_pogram_mod.Pages
{
    /// <summary>
    /// Логика взаимодействия для Autho1.xaml
    /// </summary>
    public partial class Autho1 : Page
    {
        int click;
        DispatcherTimer timer = new DispatcherTimer();
        private int seconds = 11;

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
            NavigationService.Navigate(new Client(null, null, null));
            hide_ui_captha();
        }

        private void hide_ui_captha()
        {
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
            //Debug.Print(passwordH);
            bdMod bd = new bdMod();
            
            var user = bd.users.Where(x => x.username == login && x.password == passwordH).FirstOrDefault();
            
            if (click == 1)
            {
                if (user != null)
                {
                    check_user();
                    click = 0;
                    passwordBox.Clear();
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
                    check_user();
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
                    txtLogin.Clear();
                }
            }

            if (click >= 3)
            {
                block(false);
                timer.Interval = new TimeSpan(0,0,1);
                timer.Start();
            }

            void check_user()
            {
                var user_role = bd.user_roles.Where(x => user.role_id == x.id).FirstOrDefault();

                //Если бы былм в разных таблицах:
                //switch (user_role.role)
                //{
                //    case "resident":
                //        var user_name = bd.residents.Where(x => user.id == x.user_id).FirstOrDefault();
                //        MessageBox.Show($"Привет {user_name.surname} {user_name.name}"); break;
                //    default:
                //        break;
                //}

                if (user_role.role == "admin" || user_role.role == "employee")
                {
                    var user_name = bd.employees.Where(x => user.id == x.user_id).FirstOrDefault();
                    bool isTime = hello_msg(user_name, user_role.role);
                    if (isTime)
                        LoadPage(user_role.role, user, user_name);
                    else
                        MessageBox.Show("Вы не можете войти, так как рабочий день не наступил");
                    
                }
                if (user_role.role == "resident")
                {
                    var user_name = bd.residents.Where(x => user.id == x.user_id).FirstOrDefault();
                    hello_msg(user_name, user_role.role);
                    LoadPage(user_role.role, user, user_name);
                }

                //MessageBox.Show("Вы вошли под: " + user_role.role).ToString();

            }

        }

        private bool hello_msg(employees employee_user, string role)
        {
            DateTime todayTime = DateTime.Now;

            //GetCurrentPeriod(todayTime);

            var now = DateTime.Now;
            int hour = now.Hour;
            int minute = now.Minute;
            int totalMinutes = hour * 60 + minute;

            if (totalMinutes >= 10 * 60 && totalMinutes <= 19 * 60)
            {
                if (totalMinutes >= 10 * 60 && totalMinutes <= 12 * 60)
                {
                    MessageBox.Show($"Привет: {employee_user.surname} {employee_user.name}\nВремя:Утро (10:00-12:00)");
                    return true;
                }
                else if (totalMinutes >= 12 * 60 + 1 && totalMinutes <= 17 * 60)
                {
                    MessageBox.Show($"Привет: {employee_user.surname} {employee_user.name}\nВремя:День (12:01-17:00)");
                    return true;
                }
                else if (totalMinutes >= 17 * 60 + 1 && totalMinutes <= 19 * 60)
                {
                    MessageBox.Show($"Привет: {employee_user.surname} {employee_user.name}\nВремя:Вечер (17:01-19:00)");
                    return true;
                }
            }
            else
            {
                MessageBox.Show($"Привет: {employee_user.surname} {employee_user.name}\nВне рабочего времени (10:00-19:00)");
                return false;
            }
            return true;
        }

        private void hello_msg(residents resident_user, string role)
        {
            DateTime todayTime = DateTime.Now;

            MessageBox.Show($"Привет: {resident_user.surname} {resident_user.name}\nВремя: {todayTime.Hour}:{todayTime.Minute}");

        }

        //public static string GetCurrentPeriod(DateTime date)
        //{
        //    var now = date;
        //    int hour = now.Hour;
        //    int minute = now.Minute;
        //    int totalMinutes = hour * 60 + minute;

        //    if (totalMinutes >= 10 * 60 && totalMinutes <= 19 * 60)
        //    {
        //        if (totalMinutes >= 10 * 60 && totalMinutes <= 12 * 60)
        //            return "Утро (10:00-12:00)";
        //        else if (totalMinutes >= 12 * 60 + 1 && totalMinutes <= 17 * 60)
        //            return "День (12:01-17:00)";
        //        else if (totalMinutes >= 17 * 60 + 1 && totalMinutes <= 19 * 60)
        //            return "Вечер (17:01-19:00)";
        //        else
        //            return "Рабочее время";
        //    }
        //    else
        //    {
        //        return "Вне рабочего времени (10:00-19:00)";
        //    }
        //}

        private void timer_Tick(object sender, EventArgs e)
        {
           seconds--;
           timeBlock.Content = $"Разблокировка через: {seconds.ToString()}";
            
            if (seconds <= 1)
            {
                timer.IsEnabled = false;
                //timer.Tick -= new EventHandler(timer_Tick);
                seconds = 11;
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
            btnHash.IsEnabled = blockB;
        }

        private void LoadPage(string _role, users user, employees employee)
        {
            click = 0;
            switch (_role)
            {
                case "admin":
                    NavigationService.Navigate(new Admin(user, _role, employee));
                    break;
                case "employee":
                    NavigationService.Navigate(new Employee(user, _role, employee));
                    break;
            }
        }

        private void LoadPage(string _role, users user, residents resident)
        {
            click = 0;
            switch (_role)
            {
                case "resident":
                    NavigationService.Navigate(new Client(user, _role, resident));
                    break;
            }
        }

        private void btnHash_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DiscoverHash());
            hide_ui_captha();
        }
    }
}
