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
    /// Interaction logic for SchedulePage.xaml
    /// </summary>
    public partial class SchedulePage : Page
    {
        DataTable dtSchedule;
        List<Globals.COURSE_TIMESLOT> mapTimeSlot = new List<Globals.COURSE_TIMESLOT>();
        Student myStudent;

        public SchedulePage(Student student)
        {
            InitializeComponent();
            myStudent = student;

            LoadBaseSchedule();
            LoadTermSY();

            if (cbTermSY.SelectedIndex != -1)
               LoadTimeSlot((int)cbTermSY.SelectedValue);
        }

        private void LoadTermSY()
        {
            List<TermSchoolYear> lTermSY = Db.GetTermSY(SQL.ConString, Globals.AUTH_STUDENT, myStudent.StudentID);
            cbTermSY.ItemsSource = lTermSY;
            cbTermSY.SelectedIndex = 0;
        }

        private void LoadBaseSchedule()
        {
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

        private void LoadTimeSlot(int TermSchoolYearID)
        {
            if (cbTermSY.SelectedItem != null)
            {
                List<Globals.COURSE_TIMESLOT> mapCourseTaken = Db.GetCourseEnrollment(SQL.ConString, myStudent.StudentID, TermSchoolYearID);
                List<Globals.COURSE_TIMESLOT> mapTimeSlotTemp4 = new List<Globals.COURSE_TIMESLOT>();
                mapTimeSlot.Clear();

                foreach (var item in mapCourseTaken)
                {
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

        private void cbTermSY_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadTimeSlot((int)cbTermSY.SelectedValue);
        }
    }
}
