﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace EnrollmentSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            TestConnection();
        }

        private void TestConnection()
        {
            try 
            { 
                using (SqlConnection connection = new SqlConnection(Common.CON_ACCOUNTDB))
                {
                    connection.Open();
                }
                tbStatus.Text = "SUCCESS";
            }
            catch
            {
                tbStatus.Text = "FAIL";
            }
        }
    }
}
