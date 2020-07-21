using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data;
using EnrollmentAdmin.Model;
using System.Collections.ObjectModel;
using System.Windows;

namespace EnrollmentAdmin
{
    public class Db
    {
        //SQL CONNECTION
        public static string CON_ACCOUNTDB = ConfigurationManager.ConnectionStrings["AccountDB"]?.ConnectionString;
        public static string CON_ENROLLMENTDB = ConfigurationManager.ConnectionStrings["EnrollmentDB"]?.ConnectionString;

        //GET COURSES
        public static ObservableCollection<Course> GetCourses()
        {
            try
            {
                ObservableCollection<Course> collection = new ObservableCollection<Course>();
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetCourses", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        connection.Open();
                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                collection.Add(new Course()
                                {
                                    ID = Int32.Parse(reader["ID"].ToString()),
                                    Code = reader["Code"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    Credit = float.Parse(reader["Credit"].ToString()),
                                    LectureHours = reader["LectureHours"] == DBNull.Value ? 0 : float.Parse(reader["LectureHours"].ToString()),
                                    LabHours = reader["LabHours"] == DBNull.Value ? 0 : float.Parse(reader["LabHours"].ToString())
                                });
                            }
                        }
                    }
                }
                return collection;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
        //GET SECTIONS
        public static List<Sections> GetSections(int CourseID)
        {
            try
            {
                List<Sections> collection = new List<Sections>();
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetSections", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("courseid", SqlDbType.Int).Value = CourseID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                collection.Add(new Sections()
                                {
                                    ID = Int32.Parse(reader["ID"].ToString()),
                                    Code = reader["Code"].ToString()
                                });
                            }
                        }
                    }
                }
                return collection;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
        public static List<TermSchoolYear> GetTermSY()
        {
            try
            {
                List<TermSchoolYear> collection = new List<TermSchoolYear>();
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetTermSY", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;


                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                collection.Add(new TermSchoolYear()
                                {
                                    ID = reader.GetInt32(0),
                                    TermSY = reader.GetString(1),
                                    DateStart = reader.GetDateTime(2),
                                    DateEnd = reader.GetDateTime(3)
                                });
                            }
                        }
                    }
                }
                return collection;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
    }
}
