using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTinyCollege.Models
{
    public class Instructor:Person
    {
        public DateTime HireDate { get; set; }
        public virtual ICollection<Course> Courses { get; set; }

        //Instructor to office assignment
        public virtual OfficeAssignment OfficeAssignment { get; set; }
    }
}
