using Common;
using Common.Model;
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

namespace EnrollmentStudent.View.Pages
{
    /// <summary>
    /// Interaction logic for CurriculumPage.xaml
    /// </summary>
    public partial class CurriculumPage : Page
    {
        Student myStudent;
        public CurriculumPage(Student s)
        {
            InitializeComponent();
            myStudent = s;
            LoadInfoTable();
        }

        private void LoadInfoTable()
        {
            if (cbYearLevel != null && cbTerm != null && myStudent != null)
            {
                DataTable table = Db.GetCourseCurriculum(SQL.ConString,
                    myStudent.StudentProgram.ID,
                    cbYearLevel.SelectedIndex + 1,
                    cbTerm.SelectedIndex + 1);

                dgCourseCurriculum.ItemsSource = table.DefaultView;

                lblProgram.Content = myStudent.StudentProgram.Code;
                lblYearLevel.Content = myStudent.Standing;
                lblUnitsReq.Content = myStudent.StudentProgram.Units;

                /*float totLecHrs, totLabHrs, totCredit;
                totLecHrs = table.AsEnumerable().Sum(t => t.Field<float>("LectureHours"));
                totLabHrs = table.AsEnumerable().Sum(t => t.Field<float>("LabHour"));
                totCredit = table.AsEnumerable().Sum(t => t.Field<float>("Credit"));
                lblTotalLecHrs.Content = totLecHrs.ToString();
                lblTotalLabHrs.Content = totLabHrs.ToString();
                lblTotalCredit.Content = totCredit.ToString();*/

                lblTotalLecHrs.Content = table.Compute("SUM(LectureHours)", string.Empty);
                lblTotalLabHrs.Content = table.Compute("SUM(LabHours)", string.Empty);
                lblTotalCredit.Content = table.Compute("SUM(Credit)", string.Empty);
            }
        }

        private void cbYearLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadInfoTable();
        }

        private void cbTerm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadInfoTable();
        }
    }
}
