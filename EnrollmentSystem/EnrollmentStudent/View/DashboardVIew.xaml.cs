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
using System.Windows.Shapes;

namespace EnrollmentStudent.View
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : Window
    {
        Page invalidPage = new Pages.InvalidPage();
        Student myStudent;

        public DashboardView(Student student)
        {
            InitializeComponent();
            myStudent = student;
            SelectPage(1);
        }

        private void SelectPage(int page)
        {
            switch (page)
            {
                case 1:
                    {
                        if (myStudent != null)
                        {
                            frameDashboard.Content = new Pages.ProfilePage(myStudent);
                            titleText.Text = "Profile";
                            titleIcon.Source = new BitmapImage(new Uri("pack://application:,,,/img/icon/icon-profile.png"));
                            btnCurriculum.IsEnabled = true;
                            btnEnrollment.IsEnabled = true;
                            btnSchedule.IsEnabled = true;
                            btnProfile.IsEnabled = false;
                        }
                        break;
                    }
                case 2:
                    {
                        frameDashboard.Content = new Pages.SchedulePage(myStudent);
                        titleText.Text = "My Schedule";
                        titleIcon.Source = new BitmapImage(new Uri("pack://application:,,,/img/icon/icon-schedule.png"));
                        btnCurriculum.IsEnabled = true;
                        btnEnrollment.IsEnabled = true;
                        btnSchedule.IsEnabled = false;
                        btnProfile.IsEnabled = true;
                        break;
                    }
                case 3:
                    {
                        frameDashboard.Content = new Pages.CurriculumPage(myStudent);
                        titleText.Text = "My Curriculum";
                        titleIcon.Source = new BitmapImage(new Uri("pack://application:,,,/img/icon/icon-curriculum.png"));
                        btnCurriculum.IsEnabled = false;
                        btnEnrollment.IsEnabled = true;
                        btnSchedule.IsEnabled = true;
                        btnProfile.IsEnabled = true;
                        break;
                    }
                case 4:
                    {
                        frameDashboard.Content = new Pages.EnrollmentPage(myStudent);
                        titleText.Text = "My Enrollment";
                        titleIcon.Source = new BitmapImage(new Uri("pack://application:,,,/img/icon/icon-enrollment.png"));
                        btnCurriculum.IsEnabled = true;
                        btnEnrollment.IsEnabled = false;
                        btnSchedule.IsEnabled = true;
                        btnProfile.IsEnabled = true;
                        break;
                    }
                default:
                    {
                        frameDashboard.Content = invalidPage;
                        break;
                    }
            }
        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            SelectPage(1);
        }

        private void btnSchedule_Click(object sender, RoutedEventArgs e)
        {
            SelectPage(2);
        }

        private void btnCurriculum_Click(object sender, RoutedEventArgs e)
        {
            SelectPage(3);
        }

        private void btnEnrollment_Click(object sender, RoutedEventArgs e)
        {
            SelectPage(4);
        }
    }
}
