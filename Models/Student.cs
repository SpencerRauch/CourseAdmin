#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;

namespace CourseAdmin.Models;

public class Student
{
    [Key]
    public int StudentId { get;set; }

    [Required]
    [MinLength(3)]
    public string Name { get;set; }

    public List<Enrollment> CourseEnrollments { get;set; } = new();



    
}