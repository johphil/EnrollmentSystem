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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;
using System.Data;

namespace EnrollmentStudent.View.Pages
{
    /// <summary>
    /// Interaction logic for EnrollmentPage.xaml
    /// </summary>
    public partial class EnrollmentPage : Page
    {
        Student myStudent;
        TermSchoolYear currentTermSY;
        DataTable table;
        bool HasSections = false;
        EnrollmentStatus studentEnrollmentStatus;
        int? lastSelectedStatusID;

        public EnrollmentPage(Student s)
        {
            InitializeComponent();
            myStudent = s;

            if (LoadTermSY())
                LoadCourses();

            LoadEnrollmentStatus();
            FinalizeTable();
        }

        private void LoadEnrollmentStatus()
        {
            List<EnrollmentStatus> lEnrollmentStatus = Db.GetEnrollmentStatus(SQL.ConString);
            cbEnrollmentStatus.ItemsSource = lEnrollmentStatus;

            studentEnrollmentStatus = Db.GetStudentEnrollmentStatus(SQL.ConString, myStudent.StudentID, currentTermSY.ID);

            if (studentEnrollmentStatus != null && studentEnrollmentStatus.EnrollmentStatusID == Globals.ESTATUS_ENROLLED)
            {
                gridTable.Visibility = Visibility.Hidden;
                gridTable.IsEnabled = false;
                lblNotAvailable.Visibility = Visibility.Visible;
                lblNotAvailable.Content = "You have already enrolled for this term.";
                cbEnrollmentStatus.IsEnabled = false;
                btnSections.IsEnabled = false;
            }
            else
            {
                if (studentEnrollmentStatus != null)
                {
                    cbEnrollmentStatus.SelectedValue = studentEnrollmentStatus.EnrollmentStatusID;
                    lastSelectedStatusID = studentEnrollmentStatus.EnrollmentStatusID;
                }
                else
                    cbEnrollmentStatus.SelectedIndex = -1;
            }
        }

        private bool LoadTermSY()
        {
            currentTermSY = Db.GetCurrentTermSY(SQL.ConString);

            if (currentTermSY != null)
            {
                gridTable.Visibility = Visibility.Visible;
                gridTable.IsEnabled = true;
                lblNotAvailable.Visibility = Visibility.Hidden;
                btnSections.IsEnabled = true;
                btnSections.Visibility = Visibility.Visible;
                return true;
            }
            else
            {
                gridTable.Visibility = Visibility.Hidden;
                gridTable.IsEnabled = false;
                lblNotAvailable.Visibility = Visibility.Visible;
                btnSections.IsEnabled = false;
                btnSections.Visibility = Visibility.Hidden;
                return false;
            }
        }

        private void LoadCourses()
        {
            if (myStudent != null)
            {
                table = Db.GetCourseCurriculum(SQL.ConString,
                    myStudent.StudentProgram.ID,
                    myStudent.Standing.Year,
                    currentTermSY.Term);

                DataColumn chkIncludeCol = new DataColumn("Include", typeof(bool));
                chkIncludeCol.DefaultValue = false;
                table.Columns.Add(chkIncludeCol);
                DataColumn bHasSection = new DataColumn("HasSection", typeof(bool));
                bHasSection.DefaultValue = false;
                table.Columns.Add(bHasSection);

                lblTermSY.Content = currentTermSY.TermSY;
                lblYearLevel.Content = myStudent.Standing.YearStanding;

                dgCourseCurriculum.ItemsSource = table.DefaultView;

                float totLecHrs = 0, totLabHrs = 0, totCredit = 0;

                //Select taken courses
                List<int> lTakenCoursesID = Db.GetCourseOnlyEnrollment(SQL.ConString, myStudent.StudentID, currentTermSY.ID);


                HasSections = false;

                foreach (DataRow dr in table.Rows)
                {
                    if (lTakenCoursesID.Contains(Int32.Parse(dr["CourseID"].ToString())))
                    {
                        dr["Include"] = true;
                        HasSections = true;
                    }
                    else
                    {
                        dr["Include"] = false;
                    }

                    totLecHrs += string.IsNullOrEmpty(dr["LectureHours"].ToString()) ? 0 : float.Parse(dr["LectureHours"].ToString());
                    totLabHrs += string.IsNullOrEmpty(dr["LabHours"].ToString()) ? 0 : float.Parse(dr["LabHours"].ToString());
                    totCredit += string.IsNullOrEmpty(dr["Credit"].ToString()) ? 0 : float.Parse(dr["Credit"].ToString());
                }

                lblTotalLecHrs.Content = totLecHrs.ToString();
                lblTotalLabHrs.Content = totLabHrs.ToString();
                lblTotalCredit.Content = totCredit.ToString();
            }
        }

        private void FinalizeTable()
        {
            if (lastSelectedStatusID == Globals.ESTATUS_FINALIZED)
            {
                dgCourseCurriculum.UnselectAll();
                dgCourseCurriculum.IsEnabled = false;
            }
            else
            {
                dgCourseCurriculum.IsEnabled = true;
            }
        }

        private void btnSections_Click(object sender, RoutedEventArgs e)
        {
            if (lastSelectedStatusID == Globals.ESTATUS_UNFINALIZED || lastSelectedStatusID == null)
            {
                int selected = table.AsEnumerable().Where(s => s.Field<bool>("Include") == true).Count();
                if (selected > 0)
                {
                    DataTable dtIncludedCourses = table.Clone();

                    foreach (DataRow dr in table.Rows)
                    {
                        if (dr["Include"].Equals(true))
                        {
                            dtIncludedCourses.ImportRow(dr);
                        }
                    }

                    EnrollmentSectionView esView = new EnrollmentSectionView(myStudent, currentTermSY, dtIncludedCourses);
                    esView.Owner = Application.Current.MainWindow;
                    esView.ShowInTaskbar = false;
                    esView.ShowDialog();
                    LoadCourses();
                }
                else
                {
                    MessageBox.Show("Please include a subject!");
                }
            }
            else
            {
                MessageBox.Show("You have already finalized your schedule! Please unfinalize and try again.", "Already Finalized", MessageBoxButton.OK, MessageBoxImage.Exclamation); ;
            }
        }

        private void cbEnrollmentStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbEnrollmentStatus.IsLoaded)
            {
                if (HasSections)
                {
                    if (lastSelectedStatusID != (int)cbEnrollmentStatus.SelectedValue)
                    {
                        if ((MessageBox.Show($"Do you want to {((EnrollmentStatus)cbEnrollmentStatus.SelectedItem).Description}?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)) == MessageBoxResult.Yes)
                        {
                            if (Db.UpdateStudentEnrollmentStatus(SQL.ConString, myStudent.StudentID, (int)cbEnrollmentStatus.SelectedValue, currentTermSY.ID) > 0)
                            {
                                MessageBox.Show("You have changed your enrollment status!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            lastSelectedStatusID = (int)cbEnrollmentStatus.SelectedValue;
                            FinalizeTable();
                        }
                        else
                            cbEnrollmentStatus.SelectedValue = lastSelectedStatusID;
                    }
                }
                else
                {
                    MessageBox.Show("You have not chosen section for your courses yet!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    cbEnrollmentStatus.SelectedValue = lastSelectedStatusID;
                }
            }
        }
    }
}
