#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
namespace CourseAdmin.Models;

public class SchoolContext : DbContext 
{   
    public SchoolContext(DbContextOptions options) : base(options) { }    
    public DbSet<Course> Courses { get; set; } 
    public DbSet<Enrollment> Enrollments {get;set;}
    public DbSet<Student> Students {get;set;}
    public DbSet<Subject> Subjects { get;set; }

}