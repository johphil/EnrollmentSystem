using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using EnrollmentAdmin.Model;
using Common;

namespace EnrollmentAdmin.View
{
    /// <summary>
    /// Interaction logic for NewScheduleView.xaml
    /// </summary>
    public partial class NewScheduleView : Window
    {
        TimeSlotBase SCLASS = new TimeSlotBase();
        private static DataTable DtSchedule;
        private static Course SelectedCourse = null;
        private static Schedule CourseSchedule = new Schedule();

        private static List<TermSchoolYear> lTermSY;
        private static List<Sections> lSection;

        public NewScheduleView()
        {
            InitializeComponent();
            LoadBaseSchedule();
            LoadTermSY();
        }

        private void LoadBaseSchedule()
        {
            DtSchedule = Globals.GetTable("spGetBaseSchedule", Db.CON_ENROLLMENTDB, "Schedule");
            dgTimeSlot.ItemsSource = DtSchedule.DefaultView;
        }

        private void LoadSections()
        {
            if (cbSection.Items.Count > 0)
                cbSection.Items.Clear();

            lSection = Db.GetSections(CourseSchedule);

            foreach (Sections s in lSection)
            {
                cbSection.Items.Add(s.Code);
            }

            CourseSchedule.SectionID = 0;
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
                CourseSchedule.TermSchoolYearID = lTermSY[cbTermSY.SelectedIndex].ID;
            }
        }

        private void dgTimeSlot_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (CourseSchedule.CourseID == 0 ||
                    CourseSchedule.SectionID == 0 ||
                    CourseSchedule.TermSchoolYearID == 0)
                {
                    MessageBox.Show("Please Select Course/Section and TermSY.");
                }
                else
                {

                    int time = dgTimeSlot.Items.IndexOf(dgTimeSlot.CurrentCell.Item);
                    int day = dgTimeSlot.SelectedCells[0].Column.DisplayIndex + 1;

                    string strTime = DtSchedule.Rows[time]["TimeSlot"].ToString();
                    string strDay = dgTimeSlot.SelectedCells[0].Column.Header.ToString();
                    string strDayTime = string.Format("{0} {1}", strDay, strTime);
                    string strRoom = DtSchedule.Rows[time][day].ToString();
                    string strCourseSec = string.Format("{0} / {1}", SelectedCourse.Code, cbSection.SelectedItem);
                    EditRoomView erView = new EditRoomView(strDayTime, strCourseSec, strRoom);
                    erView.ShowDialog();

                    DtSchedule.Rows[time][day] = erView.Room;
                }
                dgTimeSlot.UnselectAllCells();
            }
            catch
            {
                if (dgTimeSlot.SelectedItem != null)
                    dgTimeSlot.UnselectAllCells();
            }
        }

        private void tbCourse_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CourseLookupView clView = new CourseLookupView();
            clView.ShowDialog();
            if (clView.SelectedCourse != null)
            {
                SelectedCourse = clView.SelectedCourse;
                CourseSchedule.CourseID = clView.SelectedCourse.ID;
                tbCourse.Text = SelectedCourse.Code;
                LoadSections();
            }
        }

        private void cbSection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSection.Items.Count > 0)
                CourseSchedule.SectionID = lSection[cbSection.SelectedIndex].ID;
        }

        private void cbTermSY_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTermSY.SelectedIndex != -1)
            {
                CourseSchedule.TermSchoolYearID = lTermSY[cbTermSY.SelectedIndex].ID;
                LoadSections();
            }
        }
    }
}
