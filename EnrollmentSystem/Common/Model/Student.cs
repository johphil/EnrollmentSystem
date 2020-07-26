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
        public string Standing { get; set; }
        public Program StudentProgram { get; set; }
        public PersonInfo StudentInfo { get; set; }
    }
}
