using EnrollmentAdmin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using Common;
using EnrollmentAdmin.View;

namespace EnrollmentAdmin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            TbAccount.Focus();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            User user = Globals.Login(TbAccount.Text, TbPassword.Password, Db.CON_ENROLLMENTDB);
            if (user != null)
            {
                MessageBox.Show($"Welcome, { user.LastName }");
                ScheduleView sView = new ScheduleView();
                sView.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid Username/Password!");
            }
        }
    }
}
