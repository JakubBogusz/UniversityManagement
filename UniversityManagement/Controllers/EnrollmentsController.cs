using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UniversityManagement.Models;

namespace UniversityManagement.Controllers
{
    public class EnrollmentsController : Controller
    {
        private UniversityManagementDBEntities dbContext = new UniversityManagementDBEntities();

        // GET: Enrollments
        public async Task<ActionResult> Index()
        {
            var enrollments = dbContext.Enrollments.Include(e => e.Course).Include(e => e.Student).Include(e => e.Lecturer);
            return View(await enrollments.ToListAsync());
        }

        public PartialViewResult _enrollmentsPartial(int? courseId)
        {
            var enrollmentsData = dbContext.Enrollments.Where(x => x.CourseID == courseId)
                .Include(e => e.Course)
                .Include(e => e.Student);
            return PartialView(enrollmentsData.ToList());
        }

        // GET: Enrollments/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = await dbContext.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // GET: Enrollments/Create
        public ActionResult Create()
        {
            ViewBag.CourseID = new SelectList(dbContext.Courses, "CourseID", "Title");
            ViewBag.StudentID = new SelectList(dbContext.Students, "StudentID", "LastName");
            ViewBag.LecturerId = new SelectList(dbContext.Lecturers, "LecturerID", "FirstName");
            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EnrollmentID,Grade,CourseID,StudentID,LecturerId")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                dbContext.Enrollments.Add(enrollment);
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CourseID = new SelectList(dbContext.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewBag.StudentID = new SelectList(dbContext.Students, "StudentID", "LastName", enrollment.StudentID);
            ViewBag.LecturerId = new SelectList(dbContext.Lecturers, "LecturerID", "First_Name", enrollment.LecturerId);
            return View(enrollment);
        }

        [HttpPost]
        public async Task<JsonResult> AddStudent([Bind(Include = "CourseID, StudentID")] Enrollment enrollment)
        {
            try
            {
                var isEnrolled = dbContext.Enrollments.Any(q => q.CourseID == enrollment.CourseID && q.StudentID == enrollment.StudentID);
                if (!isEnrolled)
                {
                    if (ModelState.IsValid)
                    {
                        dbContext.Enrollments.Add(enrollment);
                        await dbContext.SaveChangesAsync();
                        return Json(new { IsSuccess = true, Message = "Students added successfully." }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { IsSuccess = false, Message = "Provided data is wrong." }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { IsSuccess = false, Message = "Students is already enrolled into course." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = "Server error. Please contact your web administrator." }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Enrollments/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = await dbContext.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseID = new SelectList(dbContext.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewBag.StudentID = new SelectList(dbContext.Students, "StudentID", "LastName", enrollment.StudentID);
            ViewBag.LecturerId = new SelectList(dbContext.Lecturers, "LecturerID", "FirstName", enrollment.LecturerId);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "EnrollmentID,Grade,CourseID,StudentID,LecturerId")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                dbContext.Entry(enrollment).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CourseID = new SelectList(dbContext.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewBag.StudentID = new SelectList(dbContext.Students, "StudentID", "LastName", enrollment.StudentID);
            ViewBag.LecturerId = new SelectList(dbContext.Lecturers, "LecturerID", "FirstName", enrollment.LecturerId);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = await dbContext.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Enrollment enrollment = await dbContext.Enrollments.FindAsync(id);
            dbContext.Enrollments.Remove(enrollment);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult GetStudents(string term)
        {
            var students = dbContext.Students.Select(x => new
            {
                Name = x.FirstName + " " + x.LastName,
                Id = x.StudentID
            }).Where(q => q.Name.Contains(term));

            return Json(students, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
