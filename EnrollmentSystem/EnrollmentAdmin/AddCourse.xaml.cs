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
using System.Windows.Shapes;

namespace EnrollmentAdmin
{
    /// <summary>
    /// Interaction logic for AddCourse.xaml
    /// </summary>
    public partial class AddCourse : Window
    {
        public AddCourse()
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {
            tbCourseCode.Clear();
            tbTitle.Clear();
            tbCredit.Clear();
            tbLabHrs.Clear();
            tbLecHrs.Clear();
            tbCourseCode.Focus();
        }

        private int InsertCourse(string Course, string Title, float Credit, float LecHrs = 0, float LabHrs = 0)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(EnrollmentAdmin.Db.CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spInsertCourse", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@code", SqlDbType.VarChar, 10).Value = Course;
                        command.Parameters.Add("@title", SqlDbType.VarChar, 64).Value = Title;
                        command.Parameters.Add("@credit", SqlDbType.Float).Value = Credit;
                        command.Parameters.Add("@lecturehours", SqlDbType.Float).Value = LecHrs;
                        command.Parameters.Add("@labhours", SqlDbType.Float).Value = LabHrs;

                        connection.Open();
                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException e)
            {
                MessageBox.Show(e.Message);
                return -1;
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                float lech, labh;
                if (string.IsNullOrEmpty(tbLecHrs.Text))
                    lech = 0;
                else
                    lech = float.Parse(tbLecHrs.Text);

                if (string.IsNullOrEmpty(tbLabHrs.Text))
                    labh = 0;
                else
                    labh = float.Parse(tbLabHrs.Text);

                if (InsertCourse(tbCourseCode.Text.ToUpper(),
                    tbTitle.Text,
                    float.Parse(tbCredit.Text),
                    lech,
                    labh) > 0)
                {
                    MessageBox.Show("Success");
                    Init();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Init();
            }
        }
    }
}
