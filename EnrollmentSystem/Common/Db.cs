using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data;
using System.Collections.ObjectModel;
using System.Windows;
using Common.Model;

namespace Common
{
    public static class Db
    {
        //DBUTILS
        public static object ToDbParameter<T>(this T? value) where T : struct
        {
            object dbValue = value;
            if (dbValue == null)
            {
                dbValue = DBNull.Value;
            }
            return dbValue;
        }
        public static object ToDbParameter(this object value)
        {
            object dbValue = value;
            if (dbValue == null)
            {
                dbValue = DBNull.Value;
            }
            return dbValue;
        }

        //LOGIN 
        public static PersonInfo Login(string CON_ENROLLMENTDB, string Account, string Password, int Auth)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spLogin", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@account", SqlDbType.VarChar, 16).Value = Account;
                        command.Parameters.Add("@password", SqlDbType.VarChar, 32).Value = Globals.MD5Hash(Password);
                        command.Parameters.Add("@member", SqlDbType.Int).Value = Auth;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new PersonInfo
                                {
                                    ID = reader.GetInt32(0),
                                    Account = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    FirstName = reader.GetString(3),
                                    MiddleName = reader.GetString(4),
                                    Gender = reader.GetString(5),
                                    DateOfBirth = reader.GetDateTime(6),
                                    HomeAddress = reader[7].ToString(),
                                    ContactNumber = reader[8].ToString()
                                };
                            }
                            else
                                return null;
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
        //GET STUDENT INFO
        public static Student GetStudentInfo(string CON_ENROLLMENTDB, PersonInfo uStudent)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetStudent", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@accountid", SqlDbType.Int).Value = uStudent.ID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Student
                                {
                                    StudentID = reader.GetInt32(0),
                                    StudentProgram = new Program
                                    {
                                        ID = reader.GetInt32(1),
                                        Code = reader.GetString(2),
                                        Description = reader.GetString(3),
                                        Units = float.Parse(reader["UnitsReq"].ToString())
                                    },
                                    Standing = reader.GetString(4),
                                    StudentInfo = uStudent
                                };
                            }
                            else
                                return null;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        //GET COURSES
        public static ObservableCollection<Course> GetCourses(string CON_ENROLLMENTDB)
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
                                    ID = reader.GetInt32(0),
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
        public static Course GetCourse(string CON_ENROLLMENTDB, int CourseID)
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
                                    ID = reader.GetInt32(0),
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
        public static List<Section> GetSections(string CON_ENROLLMENTDB)
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
                                    ID = reader.GetInt32(0),
                                    Code = reader.GetString(1)
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
        public static List<TermSchoolYear> GetTermSY(string CON_ENROLLMENTDB)
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
        public static int InsertCourseSchedule(string CON_ENROLLMENTDB, Schedule CourseSchedule)
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
        public static int UpdateCourseSchedule(string CON_ENROLLMENTDB, Schedule CourseSchedule)
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
        public static int DeleteCourseSchedule(string CON_ENROLLMENTDB, int CourseScheduleID)
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
        public static List<Globals.COURSE_SCHEDULE> GetCourseSchedules(string CON_ENROLLMENTDB, int TermID)
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
        public static Schedule GetCourseSchedule(string CON_ENROLLMENTDB, int ScheduleID)
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
        //GET PROGRAMS
        public static List<Program> GetPrograms(string CON_ENROLLMENTDB)
        {
            try
            {
                List<Program> collection = new List<Program>();
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetPrograms", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                collection.Add(new Program()
                                {
                                    ID = reader.GetInt32(0),
                                    Code = reader.GetString(1),
                                    Description = reader.GetString(2)
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
        //GET PROGRAMS
        public static List<Standing> GetStandings(string CON_ENROLLMENTDB)
        {
            try
            {
                List<Standing> collection = new List<Standing>();
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetStandings", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                collection.Add(new Standing()
                                {
                                    ID = reader.GetInt32(0),
                                    YearStanding = reader.GetString(1),
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
        //INSERT COURSE CURRICULUM
        public static int InsertCourseCurriculum(string CON_ENROLLMENTDB, Curriculum curriculum)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spInsertCourseCurriculum", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("programid", SqlDbType.Int).Value = curriculum.ProgramID;
                        command.Parameters.Add("courseid", SqlDbType.Int).Value = curriculum.CourseID;
                        command.Parameters.Add("yearlevel", SqlDbType.Int).Value = curriculum.YearLevel;
                        command.Parameters.Add("term", SqlDbType.Int).Value = curriculum.Term;
                        command.Parameters.Add("standing", SqlDbType.Int).Value = curriculum.YearStanding.ToDbParameter();
                        command.Parameters.Add("prereq", SqlDbType.VarChar).Value = curriculum.CoursePreReq;
                        command.Parameters.Add("coreq", SqlDbType.VarChar).Value = curriculum.CourseCoReq;


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
        //GET COURSE CURRICULUM 
        public static DataTable GetCourseCurriculum(string CON_ENROLLMENTDB, int ProgramID, int YearLevel, int Term)
        {
            try
            {
                DataTable table = new DataTable("CourseCurriculum");
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetCourseCurriculum", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("programid", SqlDbType.Int).Value = ProgramID;
                        command.Parameters.Add("yearlevel", SqlDbType.Int).Value = YearLevel;
                        command.Parameters.Add("term", SqlDbType.Int).Value = Term;

                        connection.Open();

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(table);
                        }
                    }
                }
                return table;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
    }
}
