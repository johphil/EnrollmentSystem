using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class CourseEnrollment
    {
        public int ID { get; set; }
        public int StudentID { get; set; }
        public int CourseScheduleID { get; set; }
        public int CourseID { get; set; }
        public int TermSchoolYearID { get; set; }
    }
}
