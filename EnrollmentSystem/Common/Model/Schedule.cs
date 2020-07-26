using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class Schedule
    {
        public int ID { get; set; }
        public int CourseID { get; set; }
        public int TermSchoolYearID { get; set; }
        public int SectionID { get; set; }
        public string Rooms { get; set; }
    }
}
