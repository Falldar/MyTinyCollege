using System;
using System.Collections.Generic;

namespace MyTinyCollege.Models
{
    public class Department
    {
        public int DepartmentID { get; set; } //PK
        public string Name { get; set; }
        public decimal Budget { get; set; }
        public DateTime StartDate { get; set; }
        public int? InstructorID { get; set; } //FK to instructor (used for headId)

        //navigation properties
        public virtual Instructor Administrator { get; set; }

        //1 department ot many courses
        public virtual ICollection<Course> Courses { get; set; }
    }
}