using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTinyCollege.Models
{
    public class Student:Person
    {
        public DateTime EnrollmentDate { get; set; }

        //1 student with many enrollment (1:M)
        public virtual ICollection<Enrollment> Enrollments { get; set; }

        /* Within the Entity Framework, the Enrollment property is called a navigation property.
         * 
         * Navigation properties hold other entities that re related to this entity. In this case,
         * the enrollments property of a student will hold all of the enrollment entities that are related
         * to that student entity.
         * 
         * In other words, if a given student row in a database has two related enrollment rows (PK-FK) that student entity's enrollment 
         * navigation property will contain those two enrollment entities.
         */

    }
}