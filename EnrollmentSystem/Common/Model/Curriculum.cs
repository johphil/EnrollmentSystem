using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class Curriculum
    {
        public int ID { get; set; }
        public int ProgramID { get; set; }
        public int CourseID { get; set; }
        public int YearLevel { get; set; }
        public int Term { get; set; }
        public int? YearStanding { get; set; }
        public string CoursePreReq { get; set; }
        public string CourseCoReq { get; set; }
        public bool IsInclude { get; set; }
    }
}
