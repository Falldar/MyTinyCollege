﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTinyCollege.Models;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MyTinyCollege.DAL
{
    class SchoolContext : DbContext
    {   

        //DAL Data Access Layer
        //Constructor: Initialize connectionstring (to match web.config)
        public SchoolContext() : base("DefaultConnection")
        {

        }

        //Specify Entity Sets - Corresponding to Database tables
        //Each single entity corresponds to a row in a table

        public DbSet<Person> People { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<Department> Departments { get; set; }

        //Specifying singular table names
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            /* Using Fluent API
             * Take care of many-to-many relationsihp between instructor and course entities. EF Code First can configure
             * this for us, but if we don't override the names we will get mapping such as 
             * InstructorInstructorID for the InstructorID column
             */
            modelBuilder.Entity<Course>()
               .HasMany(c => c.Instructors).WithMany(i => i.Courses)
               .Map(t => t.MapLeftKey("CourseID").MapRightKey("InstructorID").ToTable("CourseInstructor"));

            //The above code will create a junction (bridging) table called "CourseInstructor
            //with 2 FK columns CourseID, and InstructorID
            //CourseID -> Course
            //InstructorID -> Instructor
            //and a composite PK on CourseID + InstructorID
        }

    }
}
