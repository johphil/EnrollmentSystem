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
using static Common.Globals;

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
                                        Units = reader["UnitsReq"] == DBNull.Value ? 0 : float.Parse(reader["UnitsReq"].ToString())
                                    },
                                    Standing = new Standing
                                    {
                                        ID = reader.GetInt32(6),
                                        YearStanding = reader.GetString(4),
                                        Year = reader.GetInt32(7)
                                    },
                                    StudentInfo = uStudent,
                                    StudentNumber = reader.GetString(8)
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
        //GET COURSE SECTIONS 
        public static DataTable GetCourseSections(string CON_ENROLLMENTDB, int StudentID, int TermSchoolYearID, int CourseID)
        {
            try
            {
                DataTable table = new DataTable("CourseSections");
                table.Columns.Add("CourseScheduleID");
                table.Columns.Add("Rooms");
                table.Columns.Add("CourseID");
                table.Columns.Add("CourseCode");
                table.Columns.Add("SectionID");
                table.Columns.Add("SectionCode");
                table.Columns.Add("IsTaken", typeof(bool));

                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetCourseSections", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("studentid", SqlDbType.Int).Value = StudentID;
                        command.Parameters.Add("tsyid", SqlDbType.Int).Value = TermSchoolYearID;
                        command.Parameters.Add("courseid", SqlDbType.Int).Value = CourseID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                table.Rows.Add(new object[]
                                {
                                    reader.GetInt32(0),
                                    reader.GetString(1),
                                    reader.GetInt32(2),
                                    reader.GetString(3),
                                    reader.GetInt32(4),
                                    reader.GetString(5),
                                    reader["StudentID"] != DBNull.Value && reader.GetInt32(6) == StudentID ? true : false
                                });
                            }
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
        //GET TERM AND SCHOOLYEAR
        public static List<TermSchoolYear> GetTermSY(string CON_ENROLLMENTDB, Student student = null)
        {
            try
            {
                List<TermSchoolYear> collection = new List<TermSchoolYear>();
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetTermSY", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        if (student != null)
                            command.Parameters.Add("studentid", SqlDbType.Int).Value = student.StudentID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                collection.Add(new TermSchoolYear()
                                {
                                    ID = reader.GetInt32(0),
                                    TermSY = reader.GetString(1),
                                    Term = reader.GetInt32(2),
                                    DateStart = reader.GetDateTime(3),
                                    DateEnd = reader.GetDateTime(4)
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
        //GET CURRENT TERM SY 
        public static TermSchoolYear GetCurrentTermSY(string CON_ENROLLMENTDB)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetCurrentTermSY", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new TermSchoolYear()
                                {
                                    ID = reader.GetInt32(0),
                                    TermSY = reader.GetString(1),
                                    Term = reader.GetInt32(2),
                                    DateStart = reader.GetDateTime(3),
                                    DateEnd = reader.GetDateTime(4)
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
                                    Year = reader.GetInt32(1),
                                    YearStanding = reader.GetString(2),
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
        //INSERT COURSE ENROLLMENT
        public static int InsertCourseEnrollment(string CON_ENROLLMENTDB, CourseEnrollment courseEnrollment)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spInsertCourseEnrollment", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("studentid", SqlDbType.Int).Value = courseEnrollment.StudentID;
                        command.Parameters.Add("csid", SqlDbType.Int).Value = courseEnrollment.CourseScheduleID;
                        command.Parameters.Add("courseid", SqlDbType.Int).Value = courseEnrollment.CourseID;
                        command.Parameters.Add("tsyid", SqlDbType.Int).Value = courseEnrollment.TermSchoolYearID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader.GetInt32(0) == -5) //Already exist / taken -5 -> spInsertCourseEnrollment
                                {
                                    MessageBox.Show("This course already exist! Remove first and then assign again.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                }
                                return -1;
                            }
                            return 1;
                        }
                        //return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return -1;
            }
        }
        //GET COURSE ENROLLMENT
        public static List<COURSE_TIMESLOT> GetCourseEnrollment(string CON_ENROLLMENTDB, int StudentID, int TermSchoolYearID, bool IsRegistrar = false)
        {
            try
            {
                List<COURSE_TIMESLOT> collection = new List<COURSE_TIMESLOT>();
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetCourseEnrollment", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("studentid", SqlDbType.Int).Value = StudentID;
                        command.Parameters.Add("tsyid", SqlDbType.Int).Value = TermSchoolYearID;

                        if (IsRegistrar)
                            command.Parameters.Add("isregistrar", SqlDbType.Int).Value = 1;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (IsRegistrar)
                                {
                                    collection.Add(new COURSE_TIMESLOT()
                                    {
                                        Course = reader.GetString(0),
                                        Section = reader.GetString(1),
                                        Room = reader.GetString(2),
                                        IsConflict = false,
                                        Credit = (float)reader.GetDouble(3)
                                    });
                                }
                                else
                                {
                                    collection.Add(new COURSE_TIMESLOT()
                                    {
                                        Course = reader.GetString(0),
                                        Section = reader.GetString(1),
                                        Room = reader.GetString(2),
                                        IsConflict = false
                                    });
                                }
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
        //GET COURSE ENROLLMENT
        public static List<int> GetCourseOnlyEnrollment(string CON_ENROLLMENTDB, int StudentID, int TermSchoolYearID)
        {
            try
            {
                List<int> collection = new List<int>();
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetCourseEnrollment2", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("studentid", SqlDbType.Int).Value = StudentID;
                        command.Parameters.Add("tsyid", SqlDbType.Int).Value = TermSchoolYearID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                collection.Add(reader.GetInt32(0));
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
        //DELETE COURSE ENROLLMENT
        public static int DeleteCourseEnrollment(string CON_ENROLLMENTDB, int StudentID, int CourseID, int TermSchoolYearID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spDeleteCourseEnrollment", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("studentid", SqlDbType.Int).Value = StudentID;
                        command.Parameters.Add("courseid", SqlDbType.Int).Value = CourseID;
                        command.Parameters.Add("tsyid", SqlDbType.Int).Value = TermSchoolYearID;

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
        //INSERT OR UPDATE STUDENT ENROLLMENT STATUS
        public static int UpdateStudentEnrollmentStatus(string CON_ENROLLMENTDB, int StudentID, int EnrollmentStatusID, int TermSchoolYearID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spUpdateStudentsEnrolled", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("studentid", SqlDbType.Int).Value = StudentID;
                        command.Parameters.Add("esid", SqlDbType.Int).Value = EnrollmentStatusID;
                        command.Parameters.Add("tsyid", SqlDbType.Int).Value = TermSchoolYearID;

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
        //GET STUDENT ENROLLMENT STATUS
        public static EnrollmentStatus GetStudentEnrollmentStatus(string CON_ENROLLMENTDB, int StudentID, int TermSchoolYearID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetStudentEnrollmentStatus", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("studentid", SqlDbType.Int).Value = StudentID;
                        command.Parameters.Add("tsyid", SqlDbType.Int).Value = TermSchoolYearID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new EnrollmentStatus
                                {
                                    EnrollmentStatusID = reader.GetInt32(0),
                                    Code = reader.GetString(1),
                                    Description = reader.GetString(2)
                                };
                            }

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
        //GET ENROLLMENT STATUS
        public static List<EnrollmentStatus> GetEnrollmentStatus(string CON_ENROLLMENTDB)
        {
            try
            {
                List<EnrollmentStatus> collection = new List<EnrollmentStatus>();
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetEnrollmentStatus", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                collection.Add(new EnrollmentStatus
                                {
                                    EnrollmentStatusID = reader.GetInt32(0),
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
        //GET STUDENT ENROLLMENT INFO
        public static string[] GetStudentEnrollmentInfo(string CON_ENROLLMENTDB, string StudentNumber)
        {
            try
            {
                string[] collection = new string[5];
                using (SqlConnection connection = new SqlConnection(CON_ENROLLMENTDB))
                {
                    using (SqlCommand command = new SqlCommand("spGetStudent2", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("studentnumber", SqlDbType.Int).Value = StudentNumber;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                collection[0] = reader.GetInt32(0).ToString(); //StudentID
                                collection[1] = reader.GetString(1); //FullName
                                collection[2] = reader.GetString(2); //Program
                                collection[3] = reader.GetString(3); //Standing
                                collection[4] = reader.GetString(4); //Enrollment Status
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
