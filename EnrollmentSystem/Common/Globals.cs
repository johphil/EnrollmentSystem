using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Globals
    {
        //MEMBER AUTH
        public const int AUTH_ADMINISTRATOR = 1;
        public const int AUTH_REGISTRAR = 2;
        public const int AUTH_STUDENT = 3;

        //ERROR STRINGS
        public const string ERROR_FAIL_CONNECTION = "Connection to the database could not be established.";

        public struct COURSE_SCHEDULE
        {
            public int ID { get; set; }
            public string Course { get; set; }
            public string Section { get; set; }
            public string TermSY { get; set; }
        }
        public struct COURSE_TIMESLOT
        {
            public string Course { get; set; }
            public string Section { get; set; }
            public string Room { get; set; }
        }

        //GENERATE MD5 HASH WITH SALT
        public static string MD5Hash(string text)
        {
            text = text + ".dotnet."; //salt
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        //LOGIN 
        public static User Login(string Account, string Password, string Connection)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Connection))
                {
                    using (SqlCommand command = new SqlCommand("spLogin", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@account", SqlDbType.VarChar, 16).Value = Account;
                        command.Parameters.Add("@password", SqlDbType.VarChar, 32).Value = Globals.MD5Hash(Password);
                        command.Parameters.Add("@member", SqlDbType.Int).Value = Globals.AUTH_ADMINISTRATOR;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    ID = reader.GetInt32(0),
                                    Account = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    FirstName = reader.GetString(3),
                                    MiddleName = reader.GetString(4),
                                    Gender = reader.GetString(5),
                                    DateOfBirth = reader.GetDateTime(6),
                                    HomeAddress = reader[7].ToString(),
                                    ContactAddress = reader[8].ToString()
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

        //GET ANY TABLE USING STOREDPROC AND CONNECTION
        public static DataTable GetTable(string StoredProcedure, string SqlCon, string TableName)
        {
            try
            {
                DataTable table = new DataTable(TableName);
                using (SqlConnection connection = new SqlConnection(SqlCon))
                {
                    using (SqlCommand command = new SqlCommand(StoredProcedure, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        connection.Open();

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(table);
                        }
                    }
                }
                return table;
            }
            catch
            {
                return null;
            }
        }

        //OTHERS
        public const string TABLE_BLANK = "-";
        public const string TABLE_DELIM = ",";

        public static DataTable TableScheduleBase()
        {
            DataTable DtSchedule = new DataTable("Schedule");
            COURSE_TIMESLOT tsBlank = new COURSE_TIMESLOT()
            {
                Course = "",
                Section = "",
                Room = ""
            };

            DtSchedule.Columns.Add(new DataColumn()
            {
                ColumnName = "TimeSlot",
                DataType = typeof(string)
            });
            DtSchedule.Columns.Add(new DataColumn()
            {
                ColumnName = "Monday",
                DataType = typeof(Globals.COURSE_TIMESLOT)
            });
            DtSchedule.Columns.Add(new DataColumn()
            {
                ColumnName = "Tuesday",
                DataType = typeof(Globals.COURSE_TIMESLOT)
            });
            DtSchedule.Columns.Add(new DataColumn()
            {
                ColumnName = "Wednesday",
                DataType = typeof(Globals.COURSE_TIMESLOT)
            });
            DtSchedule.Columns.Add(new DataColumn()
            {
                ColumnName = "Thursday",
                DataType = typeof(Globals.COURSE_TIMESLOT)
            });
            DtSchedule.Columns.Add(new DataColumn()
            {
                ColumnName = "Friday",
                DataType = typeof(Globals.COURSE_TIMESLOT)
            });
            DtSchedule.Columns.Add(new DataColumn()
            {
                ColumnName = "Saturday",
                DataType = typeof(Globals.COURSE_TIMESLOT)
            });
            DtSchedule.Columns.Add(new DataColumn()
            {
                ColumnName = "Sunday",
                DataType = typeof(Globals.COURSE_TIMESLOT)
            });
            DtSchedule.Rows.Add(new object[]
            {
                "7:30 AM - \n9:00 AM",
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank
            });
            DtSchedule.Rows.Add(new object[]
            {
                "9:00 AM - \n10:30 AM",
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank
            });
            DtSchedule.Rows.Add(new object[]
            {
                "10:30 AM - \n12:00 PM",
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank
            });
            DtSchedule.Rows.Add(new object[]
            {
                "12:00 PM - \n1:30 PM",
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank
            });
            DtSchedule.Rows.Add(new object[]
            {
                "1:30 PM - \n3:00 PM",
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank
            });
            DtSchedule.Rows.Add(new object[]
            {
                "3:00 PM - \n4:30 PM",
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank
            });
            DtSchedule.Rows.Add(new object[]
            {
                "4:30 PM - \n6:00 PM",
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank
            });
            DtSchedule.Rows.Add(new object[]
            {
                "6:00 PM - \n7:30 PM",
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank
            });
            DtSchedule.Rows.Add(new object[]
            {
                "7:30 PM - \n9:00 PM",
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank,
                tsBlank
            });

            return DtSchedule;
        }
    }
}
