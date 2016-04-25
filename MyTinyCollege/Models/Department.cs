using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTinyCollege.Models
{
    public class Department
    {
        public int DepartmentID { get; set; } //PK

        [StringLength(50, MinimumLength =3)]
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]//Overriding the database column dataype (decimal -> money)
        public decimal Budget { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        public int? InstructorID { get; set; } //FK to instructor (used for headId)

        //navigation properties
        public virtual Instructor Administrator { get; set; }

        //1 department ot many courses
        public virtual ICollection<Course> Courses { get; set; }
    }
}