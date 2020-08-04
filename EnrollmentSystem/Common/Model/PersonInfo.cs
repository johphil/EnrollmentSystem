using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class PersonInfo
    {
        public int ID { get; set; }
        public string Account { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string HomeAddress { get; set; }
        public string ContactNumber { get; set; }
        public string FullName 
        { 
            get 
            {
                return $"{LastName}, {FirstName} {MiddleName}";
            } 
        }
    }
}
