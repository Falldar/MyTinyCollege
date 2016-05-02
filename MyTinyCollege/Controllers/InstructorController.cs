using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyTinyCollege.DAL;
using MyTinyCollege.Models;
using MyTinyCollege.ViewModels;

namespace MyTinyCollege.Controllers
{
    public class InstructorController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: Instructor
        public ActionResult Index(int? id, int? courseID)
        {
            //int? id       -> for determining which instructor was selected ->load related courses
            //int? courseID -> for determining which courses was selected -> load related students


            //var instructor = db.Instructors.Include(i => i.OfficeAssignment);
            //return View(instructor.ToList());

            var viewModel = new InstructorIndexData();

            //eager loading
            viewModel.Instructors = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .OrderBy(i => i.LastName);

            //check for id (an instructor has been selected -> get courses that instructor teaches)
            if (id != null)
            {
                ViewBag.InstructorID = id.Value;
                //Lazy Loading
                viewModel.Courses = viewModel.Instructors
                    .Where(i => i.ID == id.Value)
                    .Single().Courses;

                //get instructor name
                var instructorName = viewModel.Instructors.Where(i => i.ID== id.Value).Single();
                ViewBag.InstructorName = instructorName.FullName;

            }

            //check for courseid ( a course has been selected -> get enrolled students>
            if (courseID != null)
            {
                //Lazy loading
                //viewModel.Enrollments = viewModel.Courses
                //    .Where(i => i.CourseID == courseID.Value)
                //    .Single().Enrollments;

                ViewBag.CourseId = courseID.Value;

                //explicity loading
                var selectedCourse = viewModel.Courses
                    .Where(x => x.CourseID == courseID.Value)
                    .Single();
                db.Entry(selectedCourse).Collection(x => x.Enrollments).Load();
                foreach (Enrollment enrollment in selectedCourse.Enrollments)
                {
                    db.Entry(enrollment).Reference(x => x.student).Load();
                }
                viewModel.Enrollments = selectedCourse.Enrollments;

                //to send selected course title to UI
                ViewBag.CourseTitle = selectedCourse.Title;
            }

            //return the view attaching the viewModel (instructors, courses, enrollments
            return View(viewModel);
        }

        // GET: Instructor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // GET: Instructor/Create
        public ActionResult Create()
        {
            //ViewBag.ID = new SelectList(db.OfficeAssignments, "InstructorID", "Location");
            var instructor = new Instructor();
            instructor.Courses = new List<Course>();
            PopulateAssignedCourseData(instructor);
            return View();
        }

        // POST: Instructor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName,FirstName,Email,OfficeAssignment,HireDate")] Instructor instructor, string[] selectedCourses)
        {
            //check for courses checkbox selection
            if(selectedCourses != null)
            {
                instructor.Courses = new List<Course>();
                foreach (var course in selectedCourses)
                {
                    //find the selected course from UI and add it
                    var courseToAdd = db.Courses.Find(int.Parse(course));
                    instructor.Courses.Add(courseToAdd);
                }
            }
            if (ModelState.IsValid)
            {
                db.Instructors.Add(instructor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //REmember this is for our course checkboxes
            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        // GET: Instructor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Instructor instructor = db.Instructors.Find(id);
            //dbujold: replace scaffolded code to include office assignments
            //          and courses
            //          using eager loading
            Instructor instructor = db.Instructors.Include(i => i.OfficeAssignment)
                                    .Include(i => i.Courses)
                                    .Where(i => i.ID == id).Single();
            PopulateAssignedCourseData(instructor);

            if (instructor == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = new SelectList(db.OfficeAssignments, "InstructorID", "Location", instructor.ID);
            return View(instructor);
        }

        private void PopulateAssignedCourseData(Instructor instructor)
        {
            var allCourses = db.Courses;//need all courses to be displayed in view

            //populate a hash set of integers representing the course ID for each course that this instructor is teaching.
            var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.CourseID));
            var viewModel = new List<AssignedCourseData>();

            //loop all courses and see if there is a match between courses and instructor courses
            //if so, set assigned bool accordingly!
            foreach (var course in allCourses)
            {
                //instantiate and populate the assignedcoursedata object
                viewModel.Add(new AssignedCourseData{
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseID)
                });
            }

            //Populate ViewBag object with viewModel for use in UI
            ViewBag.Courses = viewModel;
            

        }

        // POST: Instructor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? ID, string[] selectedCourses)
        {

            //test for ID parameter passed in URL
            if(ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //find the instructor to update
            var instructorToUpdate = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(c => c.Courses)
                .Where(i => i.ID == ID)
                .Single();

            if(TryUpdateModel(instructorToUpdate, "", new string[] {"LastName", "FirstName", "HireDate", "OfficeAssignment", "Email"}))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment.Location))
                    {
                        instructorToUpdate.OfficeAssignment = null;
                    }
                    UpdateInstructorCourses(selectedCourses, instructorToUpdate);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Unable to saved changes. Try again later");
                }
            }

            PopulateAssignedCourseData(instructorToUpdate);
            return View(instructorToUpdate);
        }

        private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
        {
            if(selectedCourses == null)
            {
                instructorToUpdate.Courses = new List<Course>();
                return;
            }

            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var instructorCourses = new HashSet<int>(instructorToUpdate.Courses.Select(c => c.CourseID));
            
            //Loop all course in database
            foreach (var course in db.Courses)
            {
                //Add a new course to instructor assignment
                if (selectedCoursesHS.Contains(course.CourseID.ToString()))
                {
                    if (!instructorCourses.Contains(course.CourseID))
                    {
                        instructorToUpdate.Courses.Add(course);
                    }
                }
                else
                {
                    //Remove existing course to instructor assignment
                    if (instructorCourses.Contains(course.CourseID))
                    {
                        instructorToUpdate.Courses.Remove(course);
                    }
                }

                
            }
        }

        // GET: Instructor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        /* dbujold: Modify delete post method to:
         *  - Delete the office assignment record (if any) when the instructor is deleted
         *  - Remove the instructor if they are assigned as an administrator of a department
         *  Without this code, you would get a referential integrity errorif you tried to delete and 
         *  instructor who was assigned as a course administrator.
         * 
         */
        {
            //Instructor instructor = db.Instructors.Find(id);
            Instructor instructor = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Where(i => i.ID == id)
                .Single();

            //remove office assignement record for this instructor
            instructor.OfficeAssignment = null;

            //remove the instructor
            db.Instructors.Remove(instructor);

            //check for department assignment
            var department = db.Departments
                .Where(d => d.InstructorID == id)
                .SingleOrDefault(); //Use single or default here because of possible null

            if(department != null)
            {
                //remove the instrucor id value (ie. 11) for this department
                department.InstructorID = null;
            }

            //Save changes and redirect back to instructor list
            
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
