using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace EnrollmentSystem
{
    class Common
    {
        public static string CON_ACCOUNTDB = ConfigurationManager.ConnectionStrings["AccountDB"]?.ConnectionString;
        public static string CON_ENROLLMENTDB = ConfigurationManager.ConnectionStrings["EnrollmentDB"]?.ConnectionString;


        public static string ERROR_FAIL_CONNECTION = "Connection to the database could not be established.";
    }
}
