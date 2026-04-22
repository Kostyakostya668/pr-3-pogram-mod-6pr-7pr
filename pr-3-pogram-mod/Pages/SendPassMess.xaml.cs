using pr_3_pogram_mod.bd;
using pr_3_pogram_mod.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace pr_3_pogram_mod.Pages
{

    public partial class SendPassMess : Page
    {
        
        private int code;
        private users changeUser;
        private int? changeUserId = null;
        public SendPassMess()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Проверят валдиность Email, чтобы потом использовать для отправки сообщения
        /// </summary>
        /// <param name="email">Email, который будет проверяться</param>
        /// <returns>Возвращает true или false, исходся их возвраат будет понятно корректен ли Email</returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

        /// <summary>
        /// Проверяет валидность имени пользователя, чтобя по этому имени найти почту пользователя
        /// </summary>
        /// <param name="username">Имя пользователя, которое будет проверяться</param>
        /// <returns>Возвращает true или false, исходся их возвраат будет понятно корректно ли имя пользователя</returns>
        public static bool IsValidUserName(string username)
        {
            var pattern = @"^[a-zA-Z0-9_]{5,}$";

            return Regex.IsMatch(username, pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

        private void btEnterEmailOrName_Click(object sender, RoutedEventArgs e)
        {
            if (IsValidEmail(tbEmailOrName.Text))
                CheckDB(tbEmailOrName.Text, true);
            else if (IsValidUserName(tbEmailOrName.Text))
                CheckDB(tbEmailOrName.Text, false);
            else
                MessageBox.Show("Некорректные данные", "Ошибка", MessageBoxButton.OK);

        }

        /// <summary>
        /// Обращется к БД и ищет данные пользователя по имени пользоваетля или почте, если успешно вызывает метод создания сообщения
        /// </summary>
        /// <param name="checkData">Даннве по которым происходит поиск пользователя в БД (имя пользователя или почта)</param>
        /// <param name="isMail">Флаг для того чтобы понять по каким данным ползьзователя искать его в БД</param>
        private void CheckDB(string checkData, bool isMail)
        {
            using (var context = new bdMod())
            {
                if (isMail)
                {
                    changeUser = context.users.FirstOrDefault(u => u.email == checkData);

                    if (changeUser != null)
                    {
                        CreatemMail();
                    }
                    else
                    {
                        MessageBox.Show("Пользователь с такой почтой не найден", "Ошибка", MessageBoxButton.OK,MessageBoxImage.Warning);
                    }
                }
                else
                {
                    changeUser = context.users.FirstOrDefault(u => u.username == checkData);

                    if (changeUser != null)
                    {
                        CreatemMail();
                    }
                    else
                    {
                        MessageBox.Show("Пользователь с таким ником не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        /// <summary>
        /// Создает 4-значный код и вызывает метод отправки сообщения передавая код и юзера которому нужно отправить сообщение
        /// </summary>
        private void CreatemMail()
        {
            Random randomCode = new Random();
            code = randomCode.Next(1000, 10000);
            SendMail.CreateMail(changeUser.email, code, "forPas");
            changeUserId = changeUser.id;

            spCheckCode.Visibility = Visibility.Visible;
            spNameOrEmail.Visibility = Visibility.Hidden;
        }

        private void btCode_Click(object sender, RoutedEventArgs e)
        {
            if (tbCode.Text == code.ToString())
            {
                spNewPass.Visibility = Visibility.Visible;
                spCheckCode.Visibility = Visibility.Hidden;
            }
            else 
            {
                MessageBox.Show("Код неверный", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void tbCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnlyCode(tbCode);   
        }

        /// <summary>
        /// Нужен для того чтобы пользователь мог вводить только числа
        /// Вызывается при событии изменения текста в TextBox`е
        /// </summary>
        /// <param name="tbCode">Переменная для доступа к TextBox из кода </param>
        public static void OnlyCode(TextBox tbCode)
        {
            string text = new string(tbCode.Text.Where(char.IsDigit).ToArray());

            //Console.WriteLine(text);

            if (tbCode.Text != text)
            {
                tbCode.Text = text;
                tbCode.SelectionStart = tbCode.Text.Length;
            }
        }

        private void btNewPass_Click(object sender, RoutedEventArgs e)
        {
            if (tbNewPass.Text == tbNewPassAgain.Text && tbNewPassAgain.Text.Length > 5)
            {
                using (var context = new bdMod())
                {
                    if (changeUserId.HasValue)
                    {
                        var user = context.users.Find(changeUserId.Value);

                        if (user != null)
                        {
                            user.password = Services.Hash.HashPassword(tbNewPass.Text);
                            context.SaveChanges();

                            MessageBox.Show("Пароль успешно изменен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                            NavigationService.Navigate(new Pages.Autho1());
                        }
                    }

                }
            }
            else 
            {
                MessageBox.Show("Пароли не совпадают или меньше 5 символов", "Ошибка", MessageBoxButton.OK);
            }
        }
    }
}
