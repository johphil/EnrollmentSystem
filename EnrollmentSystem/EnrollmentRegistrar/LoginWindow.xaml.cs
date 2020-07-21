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
using EnrollmentRegistrar.Model;

namespace EnrollmentRegistrar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            TbAccount.Focus();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            User user = Login(TbAccount.Text, TbPassword.Password);
            if (user != null)
            {
                MessageBox.Show($"SUCCESS {user.LastName}");
            }
            else
            {
                MessageBox.Show("FAILED");
            }
        }

        private User Login(string Account, string Password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Common.CON_ACCOUNTDB))
                {
                    using (SqlCommand command = new SqlCommand("spLoginAccount", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@account", SqlDbType.VarChar, 16).Value = Account;
                        command.Parameters.Add("@password", SqlDbType.VarChar, 64).Value = Password;
                        command.Parameters.Add("@member", SqlDbType.Int).Value = Common.AUTH_REGISTRAR;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    ID = reader.GetString(0),
                                    LastName = reader.GetString(1),
                                    FirstName = reader.GetString(2),
                                    MiddleName = reader.GetString(3),
                                    Gender = reader.GetString(4),
                                    DateOfBirth = reader.GetDateTime(5),
                                    HomeAddress = reader[6].ToString(),
                                    ContactAddress = reader[7].ToString()
                                };
                            }
                            else
                                return null;
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                return null;
            }
        }
    }
}
