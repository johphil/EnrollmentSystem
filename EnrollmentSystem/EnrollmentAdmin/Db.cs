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
using Common;

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
                        using (SqlDataReader reader = command.ExecuteReader())
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
        //GET COURSE 
        public static Course GetCourse(int CourseID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetCourse", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("id", SqlDbType.Int).Value = CourseID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Course()
                                {
                                    ID = Int32.Parse(reader["ID"].ToString()),
                                    Code = reader["Code"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    Credit = float.Parse(reader["Credit"].ToString()),
                                    LectureHours = reader["LectureHours"] == DBNull.Value ? 0 : float.Parse(reader["LectureHours"].ToString()),
                                    LabHours = reader["LabHours"] == DBNull.Value ? 0 : float.Parse(reader["LabHours"].ToString())
                                };
                            }
                            else
                                return null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
        //GET SECTIONS
        public static List<Section> GetSections()
        {
            try
            {
                List<Section> collection = new List<Section>();
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetSections", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                collection.Add(new Section()
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
        //GET TERM AND SCHOOLYEAR
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
        //INSERT COURSE SCHEDULE
        public static int InsertCourseSchedule(Schedule CourseSchedule)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spInsertCourseSchedule", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("courseid", SqlDbType.Int).Value = CourseSchedule.CourseID;
                        command.Parameters.Add("tsyid", SqlDbType.Int).Value = CourseSchedule.TermSchoolYearID;
                        command.Parameters.Add("sectionid", SqlDbType.Int).Value = CourseSchedule.SectionID;
                        command.Parameters.Add("rooms", SqlDbType.VarChar).Value = CourseSchedule.Rooms;

                        connection.Open();
                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return -1;
            }
        }
        //UPDATE COURSE SCHEDULE
        public static int UpdateCourseSchedule(Schedule CourseSchedule)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spUpdateCourseSchedule", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("id", SqlDbType.Int).Value = CourseSchedule.ID;
                        command.Parameters.Add("courseid", SqlDbType.Int).Value = CourseSchedule.CourseID;
                        command.Parameters.Add("tsyid", SqlDbType.Int).Value = CourseSchedule.TermSchoolYearID;
                        command.Parameters.Add("sectionid", SqlDbType.Int).Value = CourseSchedule.SectionID;
                        command.Parameters.Add("rooms", SqlDbType.VarChar).Value = CourseSchedule.Rooms;

                        connection.Open();
                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return -1;
            }
        }
        //DELETE COURSE SCHEDULE
        public static int DeleteCourseSchedule(int CourseScheduleID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spDeleteCourseSchedule", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("id", SqlDbType.Int).Value = CourseScheduleID;

                        connection.Open();
                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return -1;
            }
        }
        //GET COURSE SCHEDULES
        public static List<Globals.COURSE_SCHEDULE> GetCourseSchedules(int TermID)
        {
            try
            {
                List<Globals.COURSE_SCHEDULE> collection = new List<Globals.COURSE_SCHEDULE>();
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetCourseSchedule", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("tsyid", SqlDbType.Int).Value = TermID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                collection.Add(new Globals.COURSE_SCHEDULE()
                                {
                                    ID = reader.GetInt32(0),
                                    Course = reader.GetString(1),
                                    Section = reader.GetString(2),
                                    TermSY = reader.GetString(3)
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
        //GET COURSE SCHEDULE
        public static Schedule GetCourseSchedule(int ScheduleID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetCourseSchedule2", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("id", SqlDbType.Int).Value = ScheduleID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Schedule()
                                {
                                    ID = reader.GetInt32(0),
                                    CourseID = reader.GetInt32(1),
                                    TermSchoolYearID = reader.GetInt32(2),
                                    SectionID = reader.GetInt32(3),
                                    Rooms = reader.GetString(4)
                                };
                            }
                            else
                                return null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
    }
}
