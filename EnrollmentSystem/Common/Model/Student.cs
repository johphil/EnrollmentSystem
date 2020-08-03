using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class Student
    {
        public int StudentID { get; set; }
        public Standing Standing { get; set; }
        public Program StudentProgram { get; set; }
        public PersonInfo StudentInfo { get; set; }
        public string StudentNumber { get; set; }
    }
}
