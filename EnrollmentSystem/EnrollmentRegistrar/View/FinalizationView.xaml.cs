using Common;
using Common.Model;
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
using System.Windows.Shapes;

namespace EnrollmentRegistrar.View
{
    /// <summary>
    /// Interaction logic for FinalizationView.xaml
    /// </summary>
    public partial class FinalizationView : Window
    {
        int StudentID, TermSchoolYearID;
        public string EnrollmentStatus;

        public FinalizationView(int StudentID, int TermSchoolYearID)
        {
            InitializeComponent();
            this.StudentID = StudentID;
            this.TermSchoolYearID = TermSchoolYearID;
            LoadEnrollmentStatus();
        }

        private void LoadEnrollmentStatus()
        {
            List<EnrollmentStatus> lEnrollmentStatus = Db.GetEnrollmentStatus(SQL.ConString);
            cbEnrollmentStatus.ItemsSource = lEnrollmentStatus;
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            if (cbEnrollmentStatus.SelectedIndex != -1)
            {
                if (Db.UpdateStudentEnrollmentStatus(SQL.ConString, StudentID, (int)cbEnrollmentStatus.SelectedValue, TermSchoolYearID) > 0)
                {
                    EnrollmentStatus = ((EnrollmentStatus)cbEnrollmentStatus.SelectedItem).Description;
                    MessageBox.Show($"You have changed the enrollment status to { EnrollmentStatus }!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
        }
    }
}
