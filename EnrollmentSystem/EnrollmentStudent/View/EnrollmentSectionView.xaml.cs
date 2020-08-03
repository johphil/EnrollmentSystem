using Common;
using Common.Model;
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

namespace EnrollmentStudent.View
{
    /// <summary>
    /// Interaction logic for EnrollmentSectionView.xaml
    /// </summary>
    public partial class EnrollmentSectionView : Window
    {
        DataTable dtSchedule;
        DataTable dtInclude;
        DataTable dtSection;
        Student myStudent;
        TermSchoolYear currentTermSY;
        int SelectedCourseScheduleID = -1;
        int SelectedCourseID = -1;
        bool isConflict = false;
        List<Globals.COURSE_TIMESLOT> mapTimeSlot = new List<Globals.COURSE_TIMESLOT>();
        List<Globals.COURSE_TIMESLOT> mapTimeSlotTemp = new List<Globals.COURSE_TIMESLOT>();
        int SelectedSectionIndex = -1;

        public EnrollmentSectionView(Student student, TermSchoolYear currentTermSY, DataTable dtInclude)
        {
            InitializeComponent();
            LoadBaseSchedule();
            myStudent = student;
            this.dtInclude = dtInclude;
            dtInclude.Columns.Add("SectionCode", typeof(string));
            this.currentTermSY = currentTermSY;
            dgIncludedCourses.ItemsSource = dtInclude.DefaultView;
            LoadTimeSlot();
        }

        private void LoadBaseSchedule()
        {
            //DtSchedule = Globals.GetTable("spGetBaseSchedule", Db.CON_ENROLLMENTDB, "Schedule");
            dtSchedule = Globals.TableScheduleBase();
            dgTimeSlot.ItemsSource = dtSchedule.DefaultView;
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

        private void CopyTimeSlot(List<Globals.COURSE_TIMESLOT> mapDest, List<Globals.COURSE_TIMESLOT> mapSrc)
        {
            if (mapDest.Count > 0 && mapSrc.Count > 0)
            {
                for (int i = 0; i < mapDest.Count; i++)
                {
                    mapDest[i] = mapSrc[i];
                }
            }

            if (mapDest.Count == 0 && mapSrc.Count > 0)
            {
                foreach (var item in mapSrc)
                {
                    mapDest.Add(item);
                }
            }
        }

        private void LoadTimeSlot()
        {
            List<Globals.COURSE_TIMESLOT> mapCourseTaken = Db.GetCourseEnrollment(SQL.ConString, myStudent.StudentID, currentTermSY.ID);
            List<Globals.COURSE_TIMESLOT> mapTimeSlotTemp4 = new List<Globals.COURSE_TIMESLOT>();
            mapTimeSlotTemp.Clear();

            foreach (var item in mapCourseTaken)
            {
                List<string> mapRooms = item.Room.Split(Globals.TABLE_DELIM.ToCharArray()).ToList();
                mapTimeSlotTemp4.Clear();
                foreach (string room in mapRooms)
                {
                    if (!string.IsNullOrWhiteSpace(item.Course))
                        SetSectionCode(item.Course, item.Section);

                    mapTimeSlotTemp4.Add(new Globals.COURSE_TIMESLOT
                    {
                        Course = string.IsNullOrWhiteSpace(room) ? "" : item.Course,
                        Section = string.IsNullOrWhiteSpace(room) ? "" : item.Section,
                        Room = room,
                        IsConflict = false
                    });
                }
                MergeTimeSlot(mapTimeSlotTemp, mapTimeSlotTemp4);
            }
            DisplayCourseTimeSlot(mapTimeSlotTemp);
        }

        private void SetSectionCode(string CourseCode, string SectionCode)
        {
            foreach (DataRow dr in dtInclude.Rows)
            {
                if (dr["CourseCode"].ToString() == CourseCode)
                {
                    dr["SectionCode"] = SectionCode;
                    dr["HasSection"] = true;
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
            }
            else
            {
                int j = 0;
                for (int day = 1; day < dtSchedule.Columns.Count; day++)
                {
                    for (int time = 0; time < dtSchedule.Rows.Count; time++)
                    {
                        dtSchedule.Rows[time][day] = new Globals.COURSE_TIMESLOT
                        {
                            Course = "",
                            Section = "",
                            Room = "",
                            IsConflict = false
                        };
                    }
                }
            }
        }

        private void dgIncludedCourses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dgSections.SelectedIndex = -1;

            if (dgIncludedCourses.SelectedIndex != -1)
            {
                int selectedCourse = Int32.Parse(dtInclude.Rows[dgIncludedCourses.SelectedIndex][9].ToString());
                dtSection = Db.GetCourseSections(SQL.ConString, myStudent.StudentID, currentTermSY.ID, selectedCourse);
                dgSections.ItemsSource = dtSection.DefaultView;

                int j = 0;
                for (int day = 1; day < dtSchedule.Columns.Count; day++)
                {
                    for (int time = 0; time < dtSchedule.Rows.Count; time++)
                    {
                        if (mapTimeSlotTemp.Count > 0)
                            dtSchedule.Rows[time][day] = mapTimeSlotTemp[j++];
                        else
                            dtSchedule.Rows[time][day] = new Globals.COURSE_TIMESLOT
                            {
                                Course = "",
                                Section = "",
                                Room = "",
                                IsConflict = false
                            };
                    }
                }

                SelectedCourseID = Int32.Parse(dtInclude.Rows[dgIncludedCourses.SelectedIndex][9].ToString());
            }
            else
            {
                SelectedCourseID = -1;
            }
        }

        private void dgSections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgIncludedCourses.SelectedIndex != -1 && dgSections.SelectedIndex != -1)
            {
                isConflict = false;
                SelectedSectionIndex = dgSections.SelectedIndex;

                DataRow selectedSection = dtSection.Rows[SelectedSectionIndex];

                List<string> mapRooms = dtSection.Rows[SelectedSectionIndex][1].ToString().Split(Globals.TABLE_DELIM.ToCharArray()).ToList();

                SelectedCourseScheduleID = Int32.Parse(dtSection.Rows[SelectedSectionIndex][0].ToString());

                List<Globals.COURSE_TIMESLOT> mapTimeSlotTemp2 = new List<Globals.COURSE_TIMESLOT>();

                mapTimeSlot.Clear();
                CopyTimeSlot(mapTimeSlot, mapTimeSlotTemp);

                int it = 0;
                foreach (string room in mapRooms)
                {
                    bool b = false;
                    if (mapTimeSlot.Count > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(mapTimeSlot[it].Room) && !string.IsNullOrWhiteSpace(room))
                        {
                            b = true;
                            isConflict = true;
                        }
                    }

                    mapTimeSlotTemp2.Add(new Globals.COURSE_TIMESLOT()
                    {
                        Course = string.IsNullOrWhiteSpace(room) ? "" : selectedSection[3].ToString(),
                        Section = string.IsNullOrWhiteSpace(room) ? "" : selectedSection[5].ToString(),
                        Room = room,
                        IsConflict = b
                    });

                    it++; 
                }

                MergeTimeSlot(mapTimeSlot, mapTimeSlotTemp2);

                DisplayCourseTimeSlot(mapTimeSlot);
            }
            else
            {
                SelectedCourseScheduleID = -1;
            }
        }

        private void btnAssign_Click(object sender, RoutedEventArgs e)
        {
            if (!isConflict)
            {
                if (Db.InsertCourseEnrollment(SQL.ConString, new CourseEnrollment
                {
                    StudentID = myStudent.StudentID,
                    CourseScheduleID = SelectedCourseScheduleID,
                    CourseID = SelectedCourseID,
                    TermSchoolYearID = currentTermSY.ID
                }) > 0)
                {
                    MergeTimeSlot(mapTimeSlotTemp, mapTimeSlot);
                    dtInclude.Rows[dgIncludedCourses.SelectedIndex]["HasSection"] = true;
                    dtInclude.Rows[dgIncludedCourses.SelectedIndex]["SectionCode"] = dtSection.Rows[dgSections.SelectedIndex]["SectionCode"];
                    dtSection.Rows[dgSections.SelectedIndex]["IsTaken"] = true;
                }
            }
            else
            {
                MessageBox.Show("Conflict schedule detected. Cannot add section.");
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            string selectedCourseCode = dtInclude.Rows[dgIncludedCourses.SelectedIndex][2].ToString();
            int selectedCourseID = Int32.Parse(dtInclude.Rows[dgIncludedCourses.SelectedIndex][9].ToString());
            if (MessageBox.Show($"Remove Section for { selectedCourseCode }?", "Remove Course Section", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                if (Db.DeleteCourseEnrollment(SQL.ConString, myStudent.StudentID, SelectedCourseID, currentTermSY.ID) > 0)
                {
                    LoadTimeSlot();
                    dtInclude.Rows[dgIncludedCourses.SelectedIndex]["HasSection"] = false;
                    dtInclude.Rows[dgIncludedCourses.SelectedIndex]["SectionCode"] = "";

                    int selectedCourse = Int32.Parse(dtInclude.Rows[dgIncludedCourses.SelectedIndex][9].ToString());
                    dtSection = Db.GetCourseSections(SQL.ConString, myStudent.StudentID, currentTermSY.ID, selectedCourse);
                    dgSections.ItemsSource = dtSection.DefaultView;
                }
            }
        }
    }
}
