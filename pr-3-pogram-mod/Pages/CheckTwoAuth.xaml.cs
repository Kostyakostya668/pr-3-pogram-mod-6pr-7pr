using pr_3_pogram_mod.bd;
using pr_3_pogram_mod.Services;
using System;
using System.Collections.Generic;
using System.Data;
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
#nullable enable

namespace pr_3_pogram_mod.Pages
{
    public partial class CheckTwoAuth : Page
    {
        private int code;

        private users user { get; set; }
        private user_roles roles { get; set; }
        private employees? employer { get; set; }
        private residents? resident { get; set; }

        public CheckTwoAuth(users useR, user_roles roleS, employees? employeR, residents? residenT)
        {
            InitializeComponent();

            this.user = useR;
            this.roles = roleS;
            this.employer = employeR;
            this.resident = residenT;

            CreateMail();
        }

        private void CreateMail()
        {
            Random random = new Random();
            code = random.Next(1000, 10000);
            SendMail.CreateMail(user.email, code);
        }

        private void btTwoAuth_Click(object sender, RoutedEventArgs e)
        {
            if (resident == null && code == Convert.ToInt32(tbTwoAuth.Text))
            {
                switch (roles.role)
                {
                    case "admin":
                        NavigationService.Navigate(new Admin(user, roles.role, employer));
                        break;
                    case "employee":
                        NavigationService.Navigate(new Employee(user, roles.role, employer));
                        break;
                }
            }
            else if (employer == null && code == Convert.ToInt32(tbTwoAuth.Text))
            {
                switch (roles.role)
                {
                    case "resident":
                        NavigationService.Navigate(new Client(user, roles.role, resident));
                        break;
                }
            }
            else
            {
                MessageBox.Show("Код неверный");
            }
        }

        private void tbTwoAuth_TextChanged(object sender, TextChangedEventArgs e)
        {
            SendPassMess.OnlyCode(tbTwoAuth);
        }
    }
}
