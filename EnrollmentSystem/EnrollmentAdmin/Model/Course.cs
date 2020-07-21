using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnrollmentAdmin.Model
{
    public class Course
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public float Credit { get; set; }
        public float LectureHours { get; set; }
        public float LabHours { get; set; }
    }
}
