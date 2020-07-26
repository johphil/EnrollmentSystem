using Common;
using Common.Model;
using EnrollmentAdmin.View;
using System.Windows;

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
            PersonInfo user = Db.Login(SQL.ConString, TbAccount.Text, TbPassword.Password, Globals.AUTH_ADMINISTRATOR);
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
