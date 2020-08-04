using Common;
using Common.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for StudentEnrollmentView.xaml
    /// </summary>
    public partial class StudentEnrollmentView : Window
    {
        TermSchoolYear currentTermSY;
        List<Globals.COURSE_TIMESLOT> mapTimeSlot = new List<Globals.COURSE_TIMESLOT>();
        DataTable dtSchedule;
        List<Course> lCourses;
        ObservableCollection<ENROLLED_COURSES> enrolledCourses = new ObservableCollection<ENROLLED_COURSES>();
        int? StudentID;

        public StudentEnrollmentView()
        {
            InitializeComponent();
            currentTermSY = Db.GetCurrentTermSY(SQL.ConString);

            if (currentTermSY == null)
            {
                MessageBox.Show("Enrollment has not yet started!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Close();
            }
            else
            {
                lblTermSY.Content = currentTermSY.TermSY;
                tbSearchStudentNumber.Focus();
                dgEnrolledCourses.ItemsSource = enrolledCourses;
            }
        }

        public class ENROLLED_COURSES
        {
            public string CourseCode { get; set; }
            public float Credit { get; set; }
            public string Section { get; set; }
            public string Room { get; set; }
            public string Schedule { get; set; }
        }

        private void LoadInfo()
        {
            string[] studentInfo = Db.GetStudentEnrollmentInfo(SQL.ConString, tbSearchStudentNumber.Text.Trim());
            if (!string.IsNullOrEmpty(studentInfo[0]))
            {
                StudentID = Int32.Parse(studentInfo[0]);
                tbStudentName.Text = studentInfo[1];
                tbStudentProgram.Text = studentInfo[2];
                tbStudentStanding.Text = studentInfo[3];
                tbEnrollmentStatus.Text = studentInfo[4];
                LoadBaseSchedule();
                LoadTimeSlot();
            }
            else
            {
                MessageBox.Show("This student has not enrolled any courses yet!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }    
        }

        private void ClearFields()
        {
            StudentID = null;
            enrolledCourses.Clear();
            dtSchedule.Clear();
            lCourses.Clear();
            mapTimeSlot.Clear();
            tbSearchStudentNumber.Clear();
            tbEnrollmentStatus.Clear();
            tbStudentName.Clear();
            tbStudentProgram.Clear();
            tbStudentStanding.Clear();
            lblTotalCredits.Content = "";
            tbSearchStudentNumber.Focus();
        }

        private void LoadBaseSchedule()
        {
            dtSchedule = Globals.TableScheduleBase();
        }

        private void LoadTimeSlot()
        {
            List<Globals.COURSE_TIMESLOT> mapCourseTaken = Db.GetCourseEnrollment(SQL.ConString, (int)StudentID, currentTermSY.ID, true);
            List<Globals.COURSE_TIMESLOT> mapTimeSlotTemp4 = new List<Globals.COURSE_TIMESLOT>();
            lCourses = new List<Course>();

            mapTimeSlot.Clear();

            foreach (var item in mapCourseTaken)
            {
                lCourses.Add(new Course
                {
                    Code = item.Course,
                    Credit = item.Credit
                });
                List<string> mapRooms = item.Room.Split(Globals.TABLE_DELIM.ToCharArray()).ToList();
                mapTimeSlotTemp4.Clear();
                foreach (string room in mapRooms)
                {
                    mapTimeSlotTemp4.Add(new Globals.COURSE_TIMESLOT
                    {
                        Course = string.IsNullOrWhiteSpace(room) ? "" : item.Course,
                        Section = string.IsNullOrWhiteSpace(room) ? "" : item.Section,
                        Room = room,
                        IsConflict = false
                    });
                }
                MergeTimeSlot(mapTimeSlot, mapTimeSlotTemp4);
            }
            DisplayCourseTimeSlot(mapTimeSlot);
        }

        private void MergeTimeSlot(List<Globals.COURSE_TIMESLOT> mapDest, List<Globals.COURSE_TIMESLOT> mapSrc)
        {
            if (mapDest.Count == 0)
            {
                foreach (Globals.COURSE_TIMESLOT item in mapSrc)
                {
                    mapDest.Add(item);
                }
            }
            else
            {
                for (int i = 0; i < mapDest.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(mapSrc[i].Room))
                    {
                        mapDest[i] = mapSrc[i];
                    }
                }
            }
        }

        private void DisplayCourseTimeSlot(List<Globals.COURSE_TIMESLOT> map)
        {
            if (map.Count > 0)
            {
                int j = 0;
                for (int day = 1; day < dtSchedule.Columns.Count; day++)
                {
                    for (int time = 0; time < dtSchedule.Rows.Count; time++)
                    {
                        dtSchedule.Rows[time][day] = map[j++];
                    }
                }
                GetSchedule();
            }
            else
            {
                for (int day = 1; day < dtSchedule.Columns.Count; day++)
                {
                    for (int time = 0; time < dtSchedule.Rows.Count; time++)
                    {
                        dtSchedule.Rows[time][day] = new Globals.COURSE_TIMESLOT
                        {
                            Course = "",
                            Section = "",
                            Room = ""
                        };
                    }
                }
            }
        }

        private void GetSchedule()
        {
            enrolledCourses.Clear();

            string[] dayofweek = new string[]
            {
                "",
                "M",
                "T",
                "W",
                "Th",
                "F",
                "S",
                "Sun"
            };

            int j = 0;
            for (int day = 1; day < dtSchedule.Columns.Count; day++)
            {
                for (int time = 0; time < dtSchedule.Rows.Count; time++)
                {
                    if (!string.IsNullOrWhiteSpace(mapTimeSlot[j].Course))
                    {
                        if (enrolledCourses.ToList().Exists(c => c.CourseCode == mapTimeSlot[j].Course)) //check if course is existing
                        {
                            for (int i = 0; i < enrolledCourses.Count; i++)
                            {
                                if (enrolledCourses[i].CourseCode == mapTimeSlot[j].Course)   //append room and schedule
                                {
                                    enrolledCourses[i].Room = enrolledCourses[i].Room + "\n" + mapTimeSlot[j].Room;
                                    string timeslot = Regex.Replace(dtSchedule.Rows[time][0].ToString(), @"\n", "");
                                    enrolledCourses[i].Schedule = enrolledCourses[i].Schedule + "\n  " + dayofweek[day] + "   " + timeslot;
                                }
                            }
                        }
                        else
                        {
                            string timeslot = Regex.Replace(dtSchedule.Rows[time][0].ToString(), @"\n", "");
                            enrolledCourses.Add(new ENROLLED_COURSES
                            {
                                CourseCode = mapTimeSlot[j].Course,
                                Section = mapTimeSlot[j].Section,
                                Room = mapTimeSlot[j].Room,
                                Schedule = "  " + dayofweek[day] + "   " + timeslot,
                                Credit = lCourses.Find(c => c.Code == mapTimeSlot[j].Course).Credit
                            });
                        }
                    }
                    j++;
                }
            }

            lblTotalCredits.Content = enrolledCourses.Sum(cr => cr.Credit).ToString();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbSearchStudentNumber.Text))
                LoadInfo();
        }

        private void btnSetEnrollmentStatus_Click(object sender, RoutedEventArgs e)
        {
            if (StudentID != null)
            {
                if (((EnrollmentStatus)Db.GetStudentEnrollmentStatus(SQL.ConString, (int)StudentID, currentTermSY.ID)).EnrollmentStatusID == Globals.ESTATUS_ENROLLED)
                {
                    MessageBox.Show("This student is already enrolled!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    tbSearchStudentNumber.Focus();
                    tbSearchStudentNumber.SelectAll();
                }
                else
                {
                    FinalizationView fView = new FinalizationView((int)StudentID, currentTermSY.ID);
                    fView.ShowDialog();
                    if (!string.IsNullOrEmpty(fView.EnrollmentStatus))
                        tbEnrollmentStatus.Text = fView.EnrollmentStatus;
                }
            }
        }

        private void btnViewSchedule_Click(object sender, RoutedEventArgs e)
        {
            ScheduleView sView = new ScheduleView(dtSchedule);
            sView.ShowDialog();
        }

        private void btnEnroll_Click(object sender, RoutedEventArgs e)
        {
            if (StudentID != null)
            {
                if (MessageBox.Show("Are you sure you want to enroll this student?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    EnrollmentStatus enrollmentStatus = Db.GetStudentEnrollmentStatus(SQL.ConString, (int)StudentID, currentTermSY.ID);
                    if (enrollmentStatus != null)
                    {
                        if (enrollmentStatus.EnrollmentStatusID == Globals.ESTATUS_UNFINALIZED)
                        {
                            MessageBox.Show("This student has not yet finalized his/her schedule!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            tbSearchStudentNumber.Focus();
                            tbSearchStudentNumber.SelectAll();
                        }
                        else if (enrollmentStatus.EnrollmentStatusID == Globals.ESTATUS_ENROLLED)
                        {
                            MessageBox.Show("This student is already enrolled!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            tbSearchStudentNumber.Focus();
                            tbSearchStudentNumber.SelectAll();
                        }
                        else
                        {
                            if (Db.UpdateStudentEnrollmentStatus(SQL.ConString, (int)StudentID, Globals.ESTATUS_ENROLLED, currentTermSY.ID) > 0)
                            {
                                string msg = string.Format("\nYou have successfully enrolled: \nName: {0} \nProgram: {1}\nTotal Units: {2}\n", tbStudentName.Text, tbStudentProgram.Text, lblTotalCredits.Content);
                                MessageBox.Show(msg, "Enroll Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                ClearFields();
                            }
                        }
                    }
                }
            }
        }
    }
}
