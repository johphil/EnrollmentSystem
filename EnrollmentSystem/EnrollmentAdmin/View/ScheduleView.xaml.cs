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

namespace EnrollmentAdmin.View
{
    /// <summary>
    /// Interaction logic for ScheduleView.xaml
    /// </summary>
    public partial class ScheduleView : Window
    {
        public ScheduleView()
        {
            InitializeComponent();
        }

        private void menuNewSchedule_Click(object sender, RoutedEventArgs e)
        {
            NewScheduleView nsView = new NewScheduleView();
            nsView.ShowDialog();
        }
    }
}
