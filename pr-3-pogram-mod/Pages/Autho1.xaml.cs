using pr_3_pogram_mod.bd;
using pr_3_pogram_mod.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Xml.Linq;

namespace pr_3_pogram_mod.Pages
{
    public partial class Autho1 : Page
    {
        private users checkUser; // конкретный пользователь, который пытается зайти

        private bool isTwoaAuthentication = false; // флаг, проводить или не проводить 2факторную аутнетификацию 
        
        int click; // счетчик нажатий чтобы вывести таймер когда нужно

        DispatcherTimer timer = new DispatcherTimer();
        private int seconds = 11;

        public Autho1()
        {
            InitializeComponent();
            capthaPanel.Visibility = Visibility.Hidden;
            timer.Tick += new EventHandler(timer_Tick); // события для того чтобы таймер работал
        }

        private void btnEnterGuest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Client(null, null, null));
            hide_ui_captha();
        }

        /// <summary>
        /// Скрывает поле капчи когда нужно
        /// </summary>
        private void hide_ui_captha()
        {
            txtBoxCaptha.Clear();
            txtBlockCaptha.Text = "Капча";
            passwordBox.Clear();
            txtLogin.Clear();
            capthaPanel.Visibility = Visibility.Hidden;
            click = 0;
        }

        /// <summary>
        /// Вызывает метод для создания капчи
        /// </summary>
        private void GenerateCapctcha()
        {
            capthaPanel.Visibility = Visibility.Visible;

            string capctchaText = CaptchaGenerator.GenerateCaptchaText(6);
            txtBlockCaptha.Text = capctchaText;
            txtBlockCaptha.TextDecorations = TextDecorations.Strikethrough;
        }

        /// <summary>
        /// Метод который проверяет есть ли пользователь, его роль нужно ли проверяет по 2FA, а также блокринкут ввод и выводит таймер
        /// </summary>
        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            click += 1;
            string login = txtLogin.Text.Trim();
            string password = passwordBox.Password.Trim();
            string passwordH = Hash.HashPassword(password); //захэшированный пароль чтобы сверить с тем что в БД
            bdMod bd = new bdMod();
            
            checkUser = bd.users.Where(x => x.username == login && x.password == passwordH).FirstOrDefault();
            
            ///Проверка кликов, чтобы понять что сейчас выводить
            if (click == 1)
            {
                if (checkUser != null)
                {
                    check_user();
                    click = 0;
                    passwordBox.Clear();
                }
                else
                {
                    MessageBox.Show("Вы ввели логин или пароль неверно");
                    GenerateCapctcha();
                    passwordBox.Clear();
                }
                txtLogin.Clear();
            }
            else if (click > 1)
            {
                if (checkUser != null && txtBoxCaptha.Text == txtBlockCaptha.Text)
                {
                    check_user();
                    capthaPanel.Visibility = Visibility.Hidden;
                    txtBoxCaptha.Clear();
                    click = 0;
                }
                else
                {
                    MessageBox.Show("Введите данные заново");
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

           /// <summary>
           /// Локальная функция для нахождения пользователя 
           /// </summary>
            void check_user()
            {
                var user_role = bd.user_roles.Where(x => checkUser.role_id == x.id).FirstOrDefault();
                // Находим роль пользователя который заходит
                if (user_role.role == "admin" || user_role.role == "employee")
                {
                    //если это работник, находит его и вызывает метод для проверки рабочий ли час сейчас
                    var user_name = bd.employees.Where(x => checkUser.id == x.user_id).FirstOrDefault();
                    bool isTime = hello_msg(user_name, user_role.role);
                    if (isTime)
                    {
                        //Если стои флаг, то переход на страницу где проходит 2FA иначе просто переходит на стрицу, которая соответствует роли пользователя
                        if (isTwoaAuthentication)
                        {
                            NavigationService.Navigate(new Pages.CheckTwoAuth(checkUser, user_role, user_name, null));   
                        }
                        else
                        {
                            LoadPage(user_role.role, checkUser, user_name);
                        }
                    }
                    else
                        MessageBox.Show("Вы не можете войти, так как рабочий день не наступил");
                    
                }
                if (user_role.role == "resident")
                {
                    //Если не работник, находит резедента и переходит к его странице 
                    var user_name = bd.residents.Where(x => checkUser.id == x.user_id).FirstOrDefault();

                    if (isTwoaAuthentication)
                    {

                    }
                    else
                    {
                        hello_msg(user_name, user_role.role);
                        LoadPage(user_role.role, checkUser, user_name);
                    }
                }
            }
        }

        /// <summary>
        /// Проверяет какое сейчас время и в зависимости от часа позволяет сотруднику войти или сообщает что сейчас не рабочее время
        /// </summary>
        /// <param name="employee_user">Конкретный сотрудник</param>
        /// <param name="role">Роль этого сотрудника</param>
        /// <returns></returns>
        private bool hello_msg(employees employee_user, string role)
        {
            DateTime todayTime = DateTime.Now;

            //GetCurrentPeriod(todayTime);

            var now = DateTime.Now;
            int hour = now.Hour;
            int minute = now.Minute;
            int totalMinutes = hour * 60 + minute;

            if (totalMinutes >= 8 * 60 && totalMinutes <= 19 * 60)
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

        /// <summary>
        /// Приветствует резидента и сообщает ему время
        /// </summary>
        /// <param name="resident_user"></param>
        /// <param name="role"></param>
        private void hello_msg(residents resident_user, string role)
        {
            DateTime todayTime = DateTime.Now;

            MessageBox.Show($"Привет: {resident_user.surname} {resident_user.name}\nВремя: {todayTime.Hour}:{todayTime.Minute}");

        }

        /// <summary>
        /// Каждый определенный тайминг вызывается и отсчитывает секунды до снятия блока
        /// </summary>
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

        /// <summary>
        /// Отключает или включает контролы для таймера
        /// </summary>
        /// <param name="blockB"></param>
        private void block(bool blockB)
        {
            timeBlock.Content = "";
            txtLogin.IsEnabled = blockB;
            passwordBox.IsEnabled = blockB;
            capthaPanel.Visibility = Visibility.Hidden;
            ButtonPanel.IsEnabled = blockB;
            btnHash.IsEnabled = blockB;
            btnMail.IsEnabled = blockB;
        }

        /// <summary>
        /// Осуществляет переход на определенную страницу исходя от сотрудника
        /// </summary>
        /// <param name="_role">Роль пользователя исходя из которой осуществляется переход</param>
        /// <param name="user">Сам юзер</param>
        /// <param name="employee">Информация о юзере</param>
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

        /// <summary>
        /// Осуществляет переход на определенную страницу резидента
        /// </summary>
        /// <param name="_role">Роль пользователя исходя из которой осуществляется переход</param>
        /// <param name="user">Сам юзер</param>
        /// <param name="resident">Инофрмация о юзере</param>
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

        private void btnMail_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SendPassMess());
            hide_ui_captha();
        }

    }
}
