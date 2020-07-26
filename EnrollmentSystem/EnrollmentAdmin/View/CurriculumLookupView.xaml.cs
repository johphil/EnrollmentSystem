using Common;
using Common.Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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
            lProgram = Db.GetPrograms(SQL.ConString);
            cbProgram.ItemsSource = lProgram;
        }

        private void LoadTable()
        {
            if (cbProgram.SelectedIndex != -1)
            {
                dgCourseCurriculum.ItemsSource = Db.GetCourseCurriculum(SQL.ConString,
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
