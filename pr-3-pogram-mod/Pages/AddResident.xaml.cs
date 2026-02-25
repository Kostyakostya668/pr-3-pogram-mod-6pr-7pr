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

        private class UsersPol
        {
            public int userId;

            [Required]
            [StringLength(20, MinimumLength = 5)]
            public string userUsername { get; set; }

            [Required]
            [StringLength(20, MinimumLength = 5)]
            public string userPassword { get; set; }

            [Required]
            [StringLength(20, MinimumLength = 5)]
            [EmailAddress(ErrorMessage = "Некорректный адрес электронной почты")]
            public string userEmail { get; set; }

            [Required]
            public int userRoleId { get; set; }

            public UsersPol(string username, string password, string email, int role_id)
            {
                userUsername = username;
                userPassword = password;
                userEmail = email;
                userRoleId = role_id;
            }
        }

        //private class Employee : UsersPol
        //{
        //    public int employeeId { set; get; }

        //    [Required]
        //    [StringLength(50, MinimumLength = 2)]
        //    public string name { set; get; }

        //    [Required]
        //    [StringLength(50, MinimumLength = 2)]
        //    public string surname { set; get; }

        //    [Required]
        //    [StringLength(75, MinimumLength = 5)]
        //    public string position { set; get; }

        //    [Required]
        //    [RegularExpression("^(\\+7|8)[\\s\\-]?\\(?\\d{3}\\)?[\\s\\-]?\\d{3}[\\s\\-]?\\d{2}[\\s\\-]?\\d{2}$")]
        //    public string phone { set; get; }

        //    [Required]
        //    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        //    public DateTime hireDate { set; get; }

        //    [Required]
        //    [StringLength(50, MinimumLength = 5)]
        //    public string departament { set; get; }

        //    public Employee(string userUsername, string userPassword, string userEmail, int userRoleId, 
        //        int id, string name, string surname, string position, string phone, DateTime hireDate, string departament) :
        //        base(userUsername, userPassword, userEmail, userRoleId)
        //    {
        //        employeeId = id;
        //        this.name = name;
        //        this.surname = surname;
        //        this.position = position;
        //        this.phone = phone;
        //        this.hireDate = hireDate;
        //        this.departament = departament;
        //    }

        //    public void Pisat()
        //    {
        //        Console.WriteLine(userId);
        //        Console.WriteLine(employeeId);
        //    }

        //}

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

            //Employee employee = new Employee(
            //        "Yeban", "govno", "dawn@gmail.com", 1488, 1001, "Ivan", "Daunov", "Kiey", "829528592", new DateTime(2021, 9, 11), "pososi hui"
            //    );
            //employee.Pisat();
            //employee.pososi();

            using (var context = new bdMod())
            {
                //var employeeList = context.employees.ToList();
                //var userList = context.users.ToList();

                //Employee employee1;

                //foreach (var itemUser in userList)
                //{
                //    foreach (var itemEmp in employeeList)
                //    {
                //        if (itemUser.id == itemEmp.user_id)
                //        {
                //            employee1 = new Employee(
                //                itemUser.id, itemUser.username, itemUser.password, itemUser.email, itemUser.role_id, 
                //                itemEmp.id, itemEmp.name, itemEmp.surname, itemEmp.position, itemEmp.phone, itemEmp.hire_date.Value, itemEmp.department
                //                );

                //            employee1.pososi();
                //        }
                //    }
                //}


                //Console.WriteLine($"Работники: {employeeList.Count}, Юзеры: {userList.Count}");

                //Employee employee1 = new Employee(
                      
                //    );
            }
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

                int selectedIndex = comboBoxNumberApart.SelectedIndex;
                if (selectedIndex >= 0 && selectedIndex < freeApartment.Count)
                {
                    newRes.apartment_id = freeApartment[selectedIndex];
                }

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

        List<int> freeApartment = new List<int>();
        List<int> freeApartmentNumber = new List<int>();

        private void btAddPol_Click_1(object sender, RoutedEventArgs e)
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
                }
            }

            comboBoxNumberApart.ItemsSource = freeApartmentNumber;

            //if (usernameBox.Text == "" || emailBox.Text == "" || passwordBox.Text == "" || comboBoxRole.SelectedIndex == -1)
            //{
            //    MessageBox.Show("Вы не ввели полную информацию", "Ошибка", MessageBoxButton.OK);

            //    return;
            //}

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
                    MessageBox.Show(newUser.password);
                    context.SaveChanges();
                    MessageBox.Show("Пользователь добавлен", "Инфо", MessageBoxButton.OK);
                }

                //newUser.email = emailBox.Text;
                //newUser.password = Services.Hash.HashPassword(passwordBox.Text);
                //newUser.role_id = 1 + comboBoxRole.SelectedIndex;
                //newUser.is_active = true;

            }

            StAddResident.IsEnabled = true;

            //Определенеи свободных комнат

        }
    }
}
