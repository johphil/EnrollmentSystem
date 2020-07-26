using Common;
using Common.Model;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EnrollmentAdmin.View
{
    /// <summary>
    /// Interaction logic for CourseLookupView.xaml
    /// </summary>
    public partial class CourseLookupView : Window
    {
        public Course SelectedCourse;
        ObservableCollection<Course> _Courses = new ObservableCollection<Course>();

        public CourseLookupView()
        {
            InitializeComponent();
            _Courses = Db.GetCourses(SQL.ConString);
            dgCourse.ItemsSource = _Courses;
            tbSearch.Focus();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (dgCourse.SelectedIndex != -1)
            {
                SelectedCourse = (Course)dgCourse.SelectedItem;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedCourse = null;
            this.Close();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            dgCourse.ItemsSource = _Courses.Where(w =>
            w.Code.Contains(tbSearch.Text.ToUpper()));
        }
    }
}
