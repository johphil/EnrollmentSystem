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
    public partial class CourseScheduleView : Window
    {
        private static DataTable DtSchedule;
        private static Course SelectedCourse = null;
        private static Schedule CourseSchedule = new Schedule();

        private static List<TermSchoolYear> lTermSY;
        private static List<Model.Section> lSection;
        private List<Globals.COURSE_TIMESLOT> mapTimeSlot = new List<Globals.COURSE_TIMESLOT>();

        private bool isEdit = false;

        //New 
        public CourseScheduleView()
        {
            InitializeComponent();
            LoadBaseSchedule();
            LoadTermSY();
            LoadSections();
        }

        //Edit
        public CourseScheduleView(string windowTitle, Schedule schedule)
        {
            InitializeComponent();
            isEdit = true;

            this.Title = windowTitle;
            CourseSchedule = schedule;

            //Load Course
            SelectedCourse = Db.GetCourse(schedule.CourseID);
            
            if (SelectedCourse != null)
                tbCourse.Text = SelectedCourse.Code;

            //Load LabHrs, LecHrs
            lblLabHrs.Content = SelectedCourse.LabHours.ToString();
            lblLecHrs.Content = SelectedCourse.LectureHours.ToString();

            //Load Section
            LoadSections();

            //Load Term
            LoadTermSY();

            //Load Schedule
            LoadBaseSchedule();

            List<string> mapRooms = CourseSchedule.Rooms.Split(Globals.TABLE_DELIM.ToCharArray()).ToList();

            foreach (string room in mapRooms)
            {
                mapTimeSlot.Add(new Globals.COURSE_TIMESLOT()
                {
                    Course = string.IsNullOrWhiteSpace(room) ? "" : SelectedCourse.Code,
                    Section = string.IsNullOrWhiteSpace(room) ? "" : lSection.Find(s => s.ID == CourseSchedule.SectionID).Code,
                    Room = room
                });
            }

            int j = 0;
            for (int day = 1; day < DtSchedule.Columns.Count; day++)
            {
                for (int time = 0; time < DtSchedule.Rows.Count; time++)
                {
                    //DtSchedule.Rows[time][day] = mapRooms[j++];
                    DtSchedule.Rows[time][day] = mapTimeSlot[j++];
                }
            }

            //Enable Remove Button
            btnRemove.IsEnabled = true;
            btnRemove.Visibility = Visibility.Visible;
        }

        private void LoadBaseSchedule()
        {
            //DtSchedule = Globals.GetTable("spGetBaseSchedule", Db.CON_ENROLLMENTDB, "Schedule");
            DtSchedule = Globals.TableScheduleBase();
            dgTimeSlot.ItemsSource = DtSchedule.DefaultView;
        }

        private void LoadSections()
        {
            //if (cbSection.Items.Count > 0)
            //    cbSection.Items.Clear();

            lSection = Db.GetSections();

            cbSection.ItemsSource = lSection;

            //foreach (Sections s in lSection)
            //{
            //    cbSection.Items.Add(s.Code);
            //}


            if (isEdit)
                cbSection.SelectedItem = lSection.Find(s => s.ID == CourseSchedule.SectionID);
            else
            {
                CourseSchedule.SectionID = 0;
            }
        }

        private void LoadTermSY()
        {
            //if (cbTermSY.Items.Count > 0)
            //    cbTermSY.Items.Clear();

            lTermSY = Db.GetTermSY();

            cbTermSY.ItemsSource = lTermSY;

            //foreach (TermSchoolYear tsy in lTermSY)
            //{
            //    cbTermSY.Items.Add(tsy.TermSY);
            //}

            if (cbTermSY.Items.Count > 0)
            {
                if (isEdit)
                {
                    cbTermSY.SelectedItem = lTermSY.Find(t => t.ID == CourseSchedule.TermSchoolYearID);
                }
                else
                {
                    cbTermSY.SelectedIndex = 0;
                    CourseSchedule.TermSchoolYearID = lTermSY[cbTermSY.SelectedIndex].ID;
                }
            }
        }

        private void CollectTimeSlotRooms()
        {
            /*foreach (DataRow row in DtSchedule.Rows)
            {
                for (int i = 2; i < DtSchedule.Columns.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(row[i].ToString()))
                        row[i] = Globals.TABLE_BLANK;
                }
            }*/

            string Monday = String.Join(Globals.TABLE_DELIM, DtSchedule.AsEnumerable().Select(x => x.Field<Globals.COURSE_TIMESLOT>("Monday".ToString()).Room).ToList());
            string Tuesday = String.Join(Globals.TABLE_DELIM, DtSchedule.AsEnumerable().Select(x => x.Field<Globals.COURSE_TIMESLOT>("Tuesday".ToString()).Room).ToList());
            string Wednesday = String.Join(Globals.TABLE_DELIM, DtSchedule.AsEnumerable().Select(x => x.Field<Globals.COURSE_TIMESLOT>("Wednesday".ToString()).Room).ToList());
            string Thursday = String.Join(Globals.TABLE_DELIM, DtSchedule.AsEnumerable().Select(x => x.Field<Globals.COURSE_TIMESLOT>("Thursday".ToString()).Room).ToList());
            string Friday = String.Join(Globals.TABLE_DELIM, DtSchedule.AsEnumerable().Select(x => x.Field<Globals.COURSE_TIMESLOT>("Friday".ToString()).Room).ToList());
            string Saturday = String.Join(Globals.TABLE_DELIM, DtSchedule.AsEnumerable().Select(x => x.Field<Globals.COURSE_TIMESLOT>("Saturday".ToString()).Room).ToList());
            string Sunday = String.Join(Globals.TABLE_DELIM, DtSchedule.AsEnumerable().Select(x => x.Field<Globals.COURSE_TIMESLOT>("Sunday".ToString()).Room).ToList());

            CourseSchedule.Rooms = Monday + Globals.TABLE_DELIM + 
                Tuesday + Globals.TABLE_DELIM + 
                Wednesday + Globals.TABLE_DELIM + 
                Thursday + Globals.TABLE_DELIM + 
                Friday + Globals.TABLE_DELIM + 
                Saturday + Globals.TABLE_DELIM + 
                Sunday;
        }

        private void UpdateTimeSlotMap()
        {
            if (CourseSchedule.CourseID > 0 &&
                CourseSchedule.SectionID > 0 &&
                CourseSchedule.TermSchoolYearID > 0 &&
                cbSection.Items.Count > 0 &&
                cbTermSY.Items.Count > 0)
            {
                int j = 0;
                for (int day = 1; day < DtSchedule.Columns.Count; day++)
                {
                    for (int time = 0; time < DtSchedule.Rows.Count; time++)
                    {
                        //DtSchedule.Rows[time][day] = mapRooms[j++];
                        Globals.COURSE_TIMESLOT ts = (Globals.COURSE_TIMESLOT)DtSchedule.Rows[time][day];
                        if (!string.IsNullOrWhiteSpace(ts.Room))
                            DtSchedule.Rows[time][day] = new Globals.COURSE_TIMESLOT()
                            {
                                Course = SelectedCourse.Code,
                                Section = lSection.Find(s => s.ID == CourseSchedule.SectionID).Code.ToString(),
                                Room = ts.Room
                            };
                    }
                }
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
                    int day = dgTimeSlot.SelectedCells[0].Column.DisplayIndex;

                    string strTime = DtSchedule.Rows[time]["TimeSlot"].ToString();
                    strTime = strTime.Replace("\n", string.Empty);

                    string strDay = dgTimeSlot.SelectedCells[0].Column.Header.ToString();
                    string strDayTime = string.Format("{0} {1}", strDay, strTime);
                    //string strRoom = DtSchedule.Rows[time][day].ToString();
                    Globals.COURSE_TIMESLOT timeslot = (Globals.COURSE_TIMESLOT)DtSchedule.Rows[time][day];
                    string strRoom = timeslot.Room;
                    string strCourseSec = string.Format("{0} / {1}", SelectedCourse.Code, lSection.Find(s => s.ID == CourseSchedule.SectionID).Code.ToString());
                    EditRoomView erView = new EditRoomView(strDayTime, strCourseSec, strRoom);
                    erView.ShowDialog();

                    //mapTimeSlot[time * (day - 2)].Room = erView.Room;
                    //DtSchedule.Rows[time][day] = erView.Room;
                    DtSchedule.Rows[time][day] = new Globals.COURSE_TIMESLOT()
                    {
                        Course = string.IsNullOrWhiteSpace(erView.Room) ? "" : SelectedCourse.Code,
                        Section = string.IsNullOrWhiteSpace(erView.Room) ? "" : lSection.Find(s => s.ID == CourseSchedule.SectionID).Code.ToString(),
                        Room = erView.Room
                    };
                    CollectTimeSlotRooms();
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
                lblLabHrs.Content = SelectedCourse.LabHours.ToString();
                lblLecHrs.Content = SelectedCourse.LectureHours.ToString();
                UpdateTimeSlotMap();
            }
        }

        private void cbSection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSection.Items.Count > 0)
            {
                CourseSchedule.SectionID = lSection[cbSection.SelectedIndex].ID;
                UpdateTimeSlotMap();
            }
        }

        private void cbTermSY_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTermSY.SelectedIndex != -1)
            {
                CourseSchedule.TermSchoolYearID = lTermSY[cbTermSY.SelectedIndex].ID;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (isEdit)
            {
                if (Db.UpdateCourseSchedule(CourseSchedule) > 0)
                {
                    MessageBox.Show("Update Success");
                    this.Close();
                }
            }
            else
            {
                if (Db.InsertCourseSchedule(CourseSchedule) > 0)
                {
                    MessageBox.Show("Save Success");
                    this.Close();
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (Db.DeleteCourseSchedule(CourseSchedule.ID) > 0)
            {
                MessageBox.Show("Remove Success");
                this.Close();
            }
        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            if (isEdit)
            {
                if (Db.UpdateCourseSchedule(CourseSchedule) > 0)
                {
                    MessageBox.Show("Update Success");
                    this.Close();
                }
            }
            else
            {
                if (Db.InsertCourseSchedule(CourseSchedule) > 0)
                {
                    MessageBox.Show("Save Success");
                    this.Close();
                }
            }
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
