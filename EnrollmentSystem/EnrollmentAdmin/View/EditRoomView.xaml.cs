using System.Windows;

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
