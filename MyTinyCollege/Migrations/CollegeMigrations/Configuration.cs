namespace MyTinyCollege.Migrations.CollegeMigrations
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MyTinyCollege.DAL.SchoolContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations\CollegeMigrations";
        }

        protected override void Seed(MyTinyCollege.DAL.SchoolContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //1. add some students
            var students = new List<Student>
            {
                new Student {FirstName = "Carson", LastName = "Alexander", EnrollmentDate=DateTime.Parse("2015-02-01"), Email="calexander@tinycollege.com" },
                new Student {FirstName = "Alonso", LastName = "Arturo", EnrollmentDate=DateTime.Parse("2015-03-01"), Email="aarturo@tinycollege.com" },
                new Student {FirstName = "John", LastName = "Smith", EnrollmentDate=DateTime.Parse("2015-05-10"), Email="jsmith@tinycollege.com" },
                new Student {FirstName = "Frank", LastName = "Bekkering", EnrollmentDate=DateTime.Parse("2015-04-20"), Email="fbekkering@tinycollege.com" },
                new Student {FirstName = "Laura", LastName = "Norman", EnrollmentDate=DateTime.Parse("2015-02-15"), Email="lnorman@tinycollege.com" }
            };

            //Loop each student and add to database ( only if they are not already present)
            //using the AddOrUpdate Method
            //The first Parameter of this method specifies the property to check if a row already exists
            students.ForEach(s => context.Students.AddOrUpdate(p => p.Email, s));
            context.SaveChanges();

            //2. add some instructors
            var instructors = new List<Instructor>
            {
                new Instructor {FirstName = "Daniel", LastName = "Bujold", HireDate = DateTime.Parse("1996-09-01"),Email="dbujold@faculty.tinycollege.com" },
                new Instructor {FirstName = "Emma", LastName = "Bujold", HireDate = DateTime.Parse("1996-08-01"),Email="ebujold@faculty.tinycollege.com" }
            };
            instructors.ForEach(s => context.Instructors.AddOrUpdate(p => p.Email, s));
            context.SaveChanges();

            //add departement
            var departments = new List<Department>
            {
                new Department {Name = "Engineering", Budget=350000, StartDate=DateTime.Parse("2010-09-01"),InstructorID=1 },
                new Department {Name = "English", Budget=150000, StartDate=DateTime.Parse("2010-09-01"),InstructorID=2 }
            };

            departments.ForEach(s => context.Departments.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();


            //3. add some course
            var courses = new List<Course>
            {
                new Course {CourseID = 1045, Title = "Chimistry", Credits=3, DepartmentID = 1},
                new Course {CourseID = 4022, Title = "Physics", Credits=3, DepartmentID = 1},
                new Course {CourseID = 3141, Title = "Calculus", Credits=3, DepartmentID = 1},
                new Course {CourseID = 2021, Title = "Literature", Credits=3, DepartmentID = 2}
            };
            courses.ForEach(s => context.Courses.AddOrUpdate(p => p.CourseID, s));
            context.SaveChanges();

            //4. add some enrollments
            var enrollments = new List<Enrollment>
            {
                new Enrollment {StudentID = 1, CourseID=1045, grade=Grade.A },
                new Enrollment {StudentID = 1, CourseID=4022, grade=Grade.B },
                new Enrollment {StudentID = 2, CourseID=3141, grade=Grade.C },
                new Enrollment {StudentID = 2, CourseID=1045, grade=Grade.B },
                new Enrollment {StudentID = 3, CourseID=2021, grade=Grade.B },
                new Enrollment {StudentID = 3, CourseID=3141, grade=Grade.C }
            };
            foreach (Enrollment e in enrollments)
            {
                var enrollmentInDatabase = context.Enrollments.Where(
                    s => s.StudentID == e.StudentID && s.CourseID == e.CourseID).SingleOrDefault();

                //SingleOrDefault:
                //  Returns a single, specific element of a sequence of values, or a default value if no such element is found.

                //Use when expecting 0 or 1 item
                //You get 0 when 0 or 1 was expected (ok)
                //you get 1 when 0 or 1 was expected (ok)
                //you get 2 or more when 0 or 1 was expected (error)

                //Single: Returns a single, specific element of a sequence
                //Use when 1 item expected (not 0 or 2 and more)
                //You get 0 when 1 was expected (error)
                //you get 1 when 1 was expected (ok)
                //you get 2 or more 1 was expected (error)

                if (enrollmentInDatabase == null)
                {
                    //enrollment was not found - add it
                    context.Enrollments.Add(e);
                }
            }
            context.SaveChanges();

        }
    }
}