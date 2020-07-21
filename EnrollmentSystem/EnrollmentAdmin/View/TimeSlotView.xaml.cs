using EnrollmentAdmin;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for TimeSlotView.xaml
    /// </summary>
    public partial class TimeSlotView : Window
    {
        public TimeSlotView()
        {
            InitializeComponent();
        }

        private void GetTimeSlot()
        {
            using (SqlConnection connection = new SqlConnection(EnrollmentAdmin.Db.CON_ACCOUNTDB))
            {
                using (SqlCommand command = new SqlCommand("spGetTimeSlot", connection))
                {

                }
            }
        }
    }
}
