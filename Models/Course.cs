using System.ComponentModel.DataAnnotations;

namespace CourseAdmin.Models;

public class Course
{
    [Key]
    public int CourseId { get;set; }

    public int SubjectId { get;set; }
    public Subject? Subject { get;set; }

    public List<Enrollment> StudentEnrollments { get;set; } = new();
}