using Common;
using Common.Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EnrollmentAdmin.View
{
    /// <summary>
    /// Interaction logic for ScheduleView.xaml
    /// </summary>
    public partial class ScheduleView : Window
    {
        private List<TermSchoolYear> lTermSY;
        private List<Globals.COURSE_SCHEDULE> lCourseSchedule;
        Globals.COURSE_SCHEDULE SelectedCourse;

        public ScheduleView()
        {
            InitializeComponent();
            LoadTermSY();
        }

        private void LoadTermSY()
        {
            if (cbTermSY.Items.Count > 0)
                cbTermSY.Items.Clear();

            lTermSY = Db.GetTermSY(SQL.ConString);

            foreach (TermSchoolYear tsy in lTermSY)
            {
                cbTermSY.Items.Add(tsy.TermSY);
            }

            if (cbTermSY.Items.Count > 0)
            {
                cbTermSY.SelectedIndex = 0;
            }
        }

        private void LoadCourseSchedules(int TermID)
        {
            lCourseSchedule = Db.GetCourseSchedules(SQL.ConString, TermID);
            if (lCourseSchedule != null)
                dgCourseSchedules.ItemsSource = lCourseSchedule;

            dgCourseSchedules.UnselectAllCells();
        }

        private void cbTermSY_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTermSY.SelectedIndex != -1)
            {
                LoadCourseSchedules(lTermSY[cbTermSY.SelectedIndex].ID);
                tbCourse.Text = "Search Course";
            }
        }

        private void tbCourse_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CourseLookupView clView = new CourseLookupView();
            clView.ShowDialog();
            if (clView.SelectedCourse != null)
            {
                dgCourseSchedules.ItemsSource = lCourseSchedule.FindAll(x => x.Course == clView.SelectedCourse.Code);
                tbCourse.Text = clView.SelectedCourse.Code;
            }
            else
            {
                dgCourseSchedules.ItemsSource = lCourseSchedule;
                tbCourse.Text = "Search Course";
            }

            dgCourseSchedules.UnselectAllCells();
        }

        private void dgCourseSchedules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCourseSchedules.SelectedIndex != -1)
            {
                SelectedCourse = (Globals.COURSE_SCHEDULE)dgCourseSchedules.SelectedItem;
                string strSelectedCourse = string.Format("{0} / {1} - {2}", SelectedCourse.Course, SelectedCourse.Section, SelectedCourse.TermSY);

                statusSelectedCourse.Content = strSelectedCourse;
            }
            else
            {
                statusSelectedCourse.Content = "";
            }
        }

        private void menuNewSchedule_Click(object sender, RoutedEventArgs e)
        {
            CourseScheduleView csView = new CourseScheduleView();
            csView.ShowDialog();
            LoadTermSY();
        }

        private void menuEditSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (dgCourseSchedules.SelectedIndex != -1)
            {
                Schedule schedule = Db.GetCourseSchedule(SQL.ConString, SelectedCourse.ID);
                CourseScheduleView csView = new CourseScheduleView(statusSelectedCourse.Content.ToString(), schedule);
                csView.ShowDialog();
                LoadTermSY();
            }
            else
            {
                MessageBox.Show("Select a course from the table.");
            }
        }

        private void btnEditCourseSchedule_Click(object sender, RoutedEventArgs e)
        {
            Globals.COURSE_SCHEDULE row = (Globals.COURSE_SCHEDULE)((Button)e.Source).DataContext;
            
            //Globals.COURSE_SCHEDULE row = (Globals.COURSE_SCHEDULE)dgCourseSchedules.SelectedItem;
            Schedule schedule = Db.GetCourseSchedule(SQL.ConString, row.ID);
            CourseScheduleView csView = new CourseScheduleView(statusSelectedCourse.Content.ToString(), schedule);
            csView.ShowDialog();
            LoadTermSY();
        }

        private void btnRemoveCourseSchedule_Click(object sender, RoutedEventArgs e)
        {
            Globals.COURSE_SCHEDULE row = (Globals.COURSE_SCHEDULE)dgCourseSchedules.SelectedItem;
            Db.DeleteCourseSchedule(SQL.ConString, row.ID);
            LoadTermSY();
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void menuLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginView lView = new LoginView();
            lView.Show();
            this.Close();
        }

        private void menuNewCurriculum_Click(object sender, RoutedEventArgs e)
        {
            AddCurriculumView acView = new AddCurriculumView();
            acView.ShowDialog();
        }

        private void menuCourseView_Click(object sender, RoutedEventArgs e)
        {
            CourseLookupView clView = new CourseLookupView();
            clView.ShowDialog();
        }

        private void menuCurriculumVIew_Click(object sender, RoutedEventArgs e)
        {
            CurriculumLookupView clView = new CurriculumLookupView();
            clView.ShowDialog();
        }
    }
}
