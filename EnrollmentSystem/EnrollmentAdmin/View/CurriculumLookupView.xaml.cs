using EnrollmentAdmin.Model;
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
    /// Interaction logic for CurriculumLookupView.xaml
    /// </summary>
    public partial class CurriculumLookupView : Window
    {
        private List<Program> lProgram;

        public CurriculumLookupView()
        {
            InitializeComponent();
            LoadPrograms();
        }

        private void LoadPrograms()
        {
            lProgram = Db.GetPrograms();
            cbProgram.ItemsSource = lProgram;
        }

        private void LoadTable()
        {
            if (cbProgram.SelectedIndex != -1)
            {
                dgCourseCurriculum.ItemsSource = Db.GetCourseCurriculum(
                    ((Program)cbProgram.SelectedItem).ID,
                    cbYearLevel.SelectedIndex + 1,
                    cbTerm.SelectedIndex + 1).DefaultView;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadTable();
        }

        private void cbYearLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadTable();
        }

        private void cbTerm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadTable();
        }
    }
}
