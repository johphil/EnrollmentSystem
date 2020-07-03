using System;
using System.Collections.Generic;
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
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using EnrollmentSystem.ViewModel;

namespace EnrollmentSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        LoginVM viewModel = new LoginVM();
        public LoginWindow()
        {
            DataContext = viewModel;

            InitializeComponent();
        }

        private int Login(string Account, string Password)
        {
            try 
            {
                using (SqlConnection connection = new SqlConnection(Common.CON_ACCOUNTDB))
                {
                    using (SqlCommand command = new SqlCommand("spLoginAccount", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@account", SqlDbType.Int).Value = Account;
                        command.Parameters.Add("@password", SqlDbType.VarChar, 64).Value = Password;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return reader.GetInt32(0);
                            }
                            else
                                return -1;
                        }
                    }
                }
            }
            catch
            {
                return -1;
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            int memberID = Login(viewModel.Account, tbPassword.Password);
            if (memberID >= 0)
            {
                string loginSuccess = $"Login Success Member ID : { memberID }";
                MessageBox.Show(loginSuccess, "SUCCESS");
            }
            else
            {
                MessageBox.Show("fAILED!", "FAILED!");
            }
        }
    }
}
