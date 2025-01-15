using System.ComponentModel.DataAnnotations;

namespace CourseAdmin.Models;

public class Enrollment
{
    [Key]
    public int EnrollmentId { get;set; }

    public int StudentId { get;set; }
    public Student? EnrolledStudent { get;set; }

    public int CourseId { get;set; }
    public Course? CourseEnrolled { get;set; }
    
}