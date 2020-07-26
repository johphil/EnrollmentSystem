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
using EnrollmentStudent;
using Common;
using EnrollmentStudent.View;
using Common.Model;

namespace EnrollmentStudent
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
            PersonInfo user = Db.Login(SQL.ConString, TbAccount.Text, TbPassword.Password, Globals.AUTH_STUDENT);
            if (user != null)
            {
                MessageBox.Show($"Welcome, { user.LastName }");

                Student s = Db.GetStudentInfo(SQL.ConString, user);
                if (s != null)
                {
                    DashboardView dView = new DashboardView(s);
                    dView.Show();
                }


                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid Username/Password!");
            }
        }
    }
}
