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
            _Courses = Db.GetCourses();
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
