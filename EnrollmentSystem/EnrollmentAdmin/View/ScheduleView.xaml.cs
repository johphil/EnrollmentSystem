using EnrollmentAdmin.Model;
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
using Common;

namespace EnrollmentAdmin.View
{
    /// <summary>
    /// Interaction logic for ScheduleView.xaml
    /// </summary>
    public partial class ScheduleView : Window
    {
        private List<TermSchoolYear> lTermSY;
        private List<Globals.COURSE_SCHEDULE> lCourseSchedule;

        public ScheduleView()
        {
            InitializeComponent();
            LoadTermSY();
            LoadCourseSchedules();
        }

        private void LoadTermSY()
        {
            if (cbTermSY.Items.Count > 0)
                cbTermSY.Items.Clear();

            lTermSY = Db.GetTermSY();

            foreach (TermSchoolYear tsy in lTermSY)
            {
                cbTermSY.Items.Add(tsy.TermSY);
            }

            if (cbTermSY.Items.Count > 0)
            {
                cbTermSY.SelectedIndex = 0;
            }
        }

        private void LoadCourseSchedules()
        {
            lCourseSchedule = Db.GetCourseSchedules();
            if (lCourseSchedule != null)
                dgCourseSchedules.ItemsSource = lCourseSchedule;
        }

        private void menuNewSchedule_Click(object sender, RoutedEventArgs e)
        {
            NewScheduleView nsView = new NewScheduleView();
            nsView.ShowDialog();
        }


    }
}
