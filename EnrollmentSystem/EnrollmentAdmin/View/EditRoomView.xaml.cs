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
using Common;

namespace EnrollmentAdmin.View
{
    /// <summary>
    /// Interaction logic for EditRoomView.xaml
    /// </summary>
    public partial class EditRoomView : Window
    {
        public string Room = "";

        public EditRoomView()
        {
            InitializeComponent();
        }

        public EditRoomView(string timeslot, string coursesec, string room)
        {
            InitializeComponent();
            lblTimeSlot.Content = timeslot;
            lblCourseSec.Content = coursesec;
            Room = room;
            
            //if (room != Globals.TABLE_BLANK)
                tbRoom.Text = room;

            tbRoom.Focus();
            tbRoom.SelectAll();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Room = tbRoom.Text.Trim();
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Room = "";
            this.Close();
        }
    }
}
