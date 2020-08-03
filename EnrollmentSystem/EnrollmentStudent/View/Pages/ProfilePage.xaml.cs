using Common.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        Student myStudent;
        public ProfilePage(Student s)
        {
            InitializeComponent();
            myStudent = s;
            LoadInfo();
        }

        private void LoadInfo()
        {
            lblStudentNumber.Content = myStudent.StudentNumber;
            tbLastName.Text = myStudent.StudentInfo.LastName;
            tbFirstName.Text = myStudent.StudentInfo.FirstName;
            tbMiddleName.Text = myStudent.StudentInfo.MiddleName;
            tbDateOfBirth.Text = myStudent.StudentInfo.DateOfBirth.ToUniversalTime().ToString("r");
            tbGender.Text = myStudent.StudentInfo.Gender;
            tbContactNumber.Text = myStudent.StudentInfo.ContactNumber;
            tbHomeAddress.Text = myStudent.StudentInfo.HomeAddress;
        }
    }
}
