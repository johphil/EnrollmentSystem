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
using EnrollmentRegistrar;
using Common;
using Common.Model;
using EnrollmentRegistrar.View;

namespace EnrollmentRegistrar
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
            PersonInfo user = Db.Login(SQL.ConString, TbAccount.Text, TbPassword.Password, Globals.AUTH_REGISTRAR);
            if (user != null)
            {
                MessageBox.Show($"Welcome, { user.FullName }", "Welcome", MessageBoxButton.OK, MessageBoxImage.Information);
                StudentEnrollmentView seView = new StudentEnrollmentView();
                seView.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid Username/Password!");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
