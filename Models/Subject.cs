#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;

namespace CourseAdmin.Models;

public class Subject
{
    [Key]
    public int SubjectId { get;set; }

    [Required]
    public string SubjectName { get;set; }
}