using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UniversityManagement.Models
{
    public class EnrollmentMetaData
    {
        public int EnrollmentID { get; set; }

        [Display(Name = "Student Grade")]
        public Nullable<decimal> Grade { get; set; }
        [Display(Name = "Course")]
        public int CourseID { get; set; }
        [Display(Name = "Course")]
        public Course Course { get; set; }

        [Display(Name = "Student")]
        public int StudentID { get; set; }
        [Display(Name = "Student")]
        public Student Student { get; set; }

        [Display(Name = "Lecturer")]
        public Nullable<int> LecturerId { get; set; }
        [Display(Name = "Lecturer")]
        public virtual Lecturer Lecturer { get; set; }
    }

    [MetadataType(typeof(EnrollmentMetaData))]
    public partial class Enrollment { }
}