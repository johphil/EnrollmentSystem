using Common;
using Common.Model;
using EnrollmentAdmin.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EnrollmentAdmin
{
    /// <summary>
    /// Interaction logic for CurriculumAdd.xaml
    /// </summary>
    public partial class AddCurriculumView : Window
    {
        private List<Program> lProgram;
        private List<Standing> lStanding;
        private Course SelectedCourse;
        private ObservableCollection<Course> lCoursePreReq = new ObservableCollection<Course>();
        private ObservableCollection<Course> lCourseCoReq = new ObservableCollection<Course>();

        public AddCurriculumView()
        {
            InitializeComponent();
            LoadPrograms();
            LoadStandings();
            dgPreReq.ItemsSource = lCoursePreReq;
            dgCoReq.ItemsSource = lCourseCoReq;
        }

        private void LoadPrograms()
        {
            lProgram = Db.GetPrograms(SQL.ConString);
            cbProgram.ItemsSource = lProgram;
        }

        private void LoadStandings()
        {
            lStanding = Db.GetStandings(SQL.ConString);
            cbStanding.ItemsSource = lStanding;
        }
    
        private Course SelectCourse()
        {
            CourseLookupView clView = new CourseLookupView();
            clView.ShowDialog();
            if (clView.SelectedCourse != null)
                return clView.SelectedCourse;
            else
                return null;
        }

        private void tbCourse_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectedCourse = SelectCourse();
            if (SelectedCourse != null)
            {
                tbCourse.Text = SelectedCourse.Code;
                lblCourseDescription.Content = SelectedCourse.Description.ToString();
                lblCourseUnits.Content = "Units: " + SelectedCourse.Credit.ToString();
            }
        }

        private void btnAddPreReq_Click(object sender, RoutedEventArgs e)
        {
            Course c = SelectCourse();
            if (c != null)
            {
                lCoursePreReq.Add(c);
            }
        }

        private void btnAddCoReq_Click(object sender, RoutedEventArgs e)
        {
            Course c = SelectCourse();
            if (c != null)
            {
                lCourseCoReq.Add(c);
            }
        }

        private void btnRemoveCoursePreReq_Click(object sender, RoutedEventArgs e)
        {
            Course row = (Course)dgPreReq.SelectedItem;
            lCoursePreReq.Remove(row);
        }

        private void btnRemoveCourseCoReq_Click(object sender, RoutedEventArgs e)
        {
            Course row = (Course)dgCoReq.SelectedItem;
            lCourseCoReq.Remove(row);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cbProgram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Program program = (Program)cbProgram.SelectedItem;

            if (program != null)
            {
                lblProgram.Content = program.Description;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string strCoursePreReq = null;
            string strCourseCoReq = null;

            if (lCoursePreReq.Count > 0)
            {
                strCoursePreReq = String.Join(Globals.TABLE_DELIM, lCoursePreReq.AsEnumerable().Select(x => x.ID).ToList());
            }

            if (lCourseCoReq.Count > 0)
            {
                strCourseCoReq = String.Join(Globals.TABLE_DELIM, lCourseCoReq.AsEnumerable().Select(x => x.ID).ToList());
            }

            Curriculum coursecurriculum = new Curriculum
            {
                ProgramID = ((Program)cbProgram.SelectedItem).ID,
                CourseID = SelectedCourse.ID,
                YearLevel = cbYearLevel.SelectedIndex + 1,
                Term = cbTerm.SelectedIndex + 1,
                YearStanding = cbStanding.SelectedIndex == -1 ? (int?)null : ((Standing)cbStanding.SelectedItem).ID,
                CoursePreReq = strCoursePreReq,
                CourseCoReq = strCourseCoReq
            };

            if (Db.InsertCourseCurriculum(SQL.ConString, coursecurriculum) > 0)
            {
                MessageBox.Show("Insert Success");
                SelectedCourse = null;
                tbCourse.Text = "";
                lblCourseDescription.Content = "";
                lblCourseUnits.Content = "";
                lCoursePreReq.Clear();
                lCourseCoReq.Clear();
                cbStanding.SelectedIndex = -1;
            }
        }
    }
}
