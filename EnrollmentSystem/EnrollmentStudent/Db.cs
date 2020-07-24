using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace EnrollmentStudent
{
    public static class Db
    {
        //SQL CONNECTION
        public static string CON_ACCOUNTDB = ConfigurationManager.ConnectionStrings["AccountDB"]?.ConnectionString;
        public static string CON_ENROLLMENTDB = ConfigurationManager.ConnectionStrings["EnrollmentDB"]?.ConnectionString;

        //MEMBER AUTH
        public static int AUTH_ADMINISTRATOR = 1;
        public static int AUTH_REGISTRAR = 2;
        public static int AUTH_STUDENT = 3;

        //ERROR STRINGS
        public static string ERROR_FAIL_CONNECTION = "Connection to the database could not be established.";
    }
}
