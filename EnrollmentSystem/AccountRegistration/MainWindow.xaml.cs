using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace AccountRegistration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string CON_ENROLLMENTDB = ConfigurationManager.ConnectionStrings["EnrollmentDB"]?.ConnectionString;
        public MainWindow()
        {
            InitializeComponent();
            cbProgram.ItemsSource = GetPrograms();
        }

        public struct PROGRAM
        {
            public int ID { get; set; }
            public string Code { get; set; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Register() > 0)
            {
                MessageBox.Show(string.Format("The account {0} has been created.", tbAccount.Text));
            }
            else
            {
                MessageBox.Show("Error");
            }
        }

        private int Register()
        {
            try
            {
                string account, password, lastname, firstname, middlename, address, contact;
                account = tbAccount.Text;
                password = tbPassword.Text;
                lastname = tbLastName.Text;
                firstname = tbFirstName.Text;
                middlename = tbMiddleName.Text;
                int memberid, genderid;
                memberid = cbAuth.SelectedIndex + 1;
                genderid = cbGender.SelectedIndex + 1;
                DateTime dob = dpBirthday.SelectedDate.HasValue ? dpBirthday.SelectedDate.Value : DateTime.MinValue;
                address = tbAddress.Text;
                contact = tbContact.Text;

                if (string.IsNullOrWhiteSpace(account)
                    || string.IsNullOrEmpty(password)
                    || string.IsNullOrEmpty(lastname)
                    || string.IsNullOrEmpty(firstname)
                    || string.IsNullOrEmpty(middlename)
                    || memberid < 1
                    || genderid < 1
                    || dob == DateTime.MinValue)
                {
                    MessageBox.Show("Invalid. Please fill out all fields.");
                    return -1;
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                    {
                        using (SqlCommand command = new SqlCommand("spRegister", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@account", account);
                            command.Parameters.AddWithValue("@password", Globals.MD5Hash(password));
                            command.Parameters.AddWithValue("@memberid", memberid);
                            command.Parameters.AddWithValue("@lastname", lastname);
                            command.Parameters.AddWithValue("@firstname", firstname);
                            command.Parameters.AddWithValue("@middlename", middlename);
                            command.Parameters.AddWithValue("@genderid", genderid);
                            command.Parameters.AddWithValue("@dob", dob);
                            command.Parameters.AddWithValue("@address", address);
                            command.Parameters.AddWithValue("@contact", contact);

                            if (cbProgram.SelectedIndex == -1)
                                command.Parameters.AddWithValue("@programid", DBNull.Value);
                            else
                                command.Parameters.AddWithValue("@programid", ((PROGRAM)cbProgram.SelectedItem).ID);

                            connection.Open();
                            return command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                MessageBox.Show(e.Message);
                return -1;
            }
        }

        public static List<PROGRAM> GetPrograms()
        {
            try
            {
                List<PROGRAM> collection = new List<PROGRAM>();
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetPrograms", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                collection.Add(new PROGRAM()
                                {
                                    ID = reader.GetInt32(0),
                                    Code = reader.GetString(1)
                                });
                            }
                        }
                    }
                }
                return collection;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
    }
}
