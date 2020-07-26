using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnrollmentAdmin
{
    public class SQL
    {
        public static string ConString
        {
            get { return ConfigurationManager.ConnectionStrings["EnrollmentDB"]?.ConnectionString; }
        }
    }
}
