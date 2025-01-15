using System.ComponentModel.DataAnnotations;

namespace CourseAdmin.Models;

public class Course
{
    [Key]
    public int CourseId { get;set; }

    //fk
    public int SubjectId { get;set; }
    //nav prop
    public Subject? Subject { get;set; }
    //nav prop
    public List<Enrollment> StudentEnrollments { get;set; } = new();
}