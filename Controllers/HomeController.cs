using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CourseAdmin.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseAdmin.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private SchoolContext _context;

    public HomeController(ILogger<HomeController> logger, SchoolContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        ViewAdminPanel viewModel = new()
        {
            AllCourses = _context.Courses.Include(c => c.StudentEnrollments).ThenInclude(e=>e.EnrolledStudent).ToList(),
            AllStudents = _context.Students.Include(s => s.CourseEnrollments).ThenInclude(e => e.CourseEnrolled).ToList(),
            AllSubjects = _context.Subjects.ToList()
        };
        //explicit with View name because we will rerun this method to render this view with model errors
        // in CreateStudent, CreateSubject and CreateCourse without having to reform viewModel in those routes
        return View("Index", viewModel);
    }

    public IActionResult CreateStudent(Student newStudent)
    {
        if (!ModelState.IsValid)
        {
            return Index();
        }
        _context.Add(newStudent);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    public IActionResult CreateSubject(Subject newSubject)
    {
        if (!ModelState.IsValid)
        {
            return Index();
        }
        _context.Add(newSubject);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public IActionResult CreateCourse(ViewCourseForm courseForm)
    {
        // this commented out code is useful for seeing what validation is failing
        // if (!ModelState.IsValid)
        // {
        //     var message = string.Join(" | ", ModelState.Values
        //         .SelectMany(v => v.Errors)
        //         .Select(e => e.ErrorMessage));
        //     Console.WriteLine(message);
        // }
        if (!ModelState.IsValid)
        {
            return Index();
        }
        Course newCourse = new() { SubjectId = courseForm.SubjectId };
        _context.Add(newCourse);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    //gets courseID from form, toggles
    //used in _StudentTable
    [HttpPost("enrollments/{studentID}")]
    public IActionResult ToggleEnroll(int studentID, int courseID)
    {
        Enrollment? existingEnrollment = _context.Enrollments.FirstOrDefault(e => e.StudentId == studentID && e.CourseId == courseID);
        if (existingEnrollment == null)
        {
            Enrollment newEnroll = new() { StudentId = studentID, CourseId = courseID };
            _context.Add(newEnroll);
        }
        else
        {
            _context.Remove(existingEnrollment);
        }
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    //gets courseID from route, only drops
    //used in _StudentDetail
    [HttpPost("enrollments/{studentID}/{courseID}/drop")]
    public RedirectToActionResult DropEnroll(int studentID, int courseID)
    {
        Enrollment? existingEnrollment = _context.Enrollments.FirstOrDefault(e => e.StudentId == studentID && e.CourseId == courseID);
        if (existingEnrollment != null)
        {
            _context.Remove(existingEnrollment);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }

    //gets courseID from route, only adds
    //unused
    [HttpPost("enrollments/{studentID}/{courseID}/add")]
    public RedirectToActionResult AddEnroll(int studentID, int courseID)
    {
        Enrollment? existingEnrollment = _context.Enrollments.FirstOrDefault(e => e.StudentId == studentID && e.CourseId == courseID);
        if (existingEnrollment == null)
        {
            Enrollment newEnroll = new(){CourseId=courseID,StudentId=studentID};
            _context.Add(newEnroll);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }

    //sets selected studentID into session after confirming existence of student
    [HttpPost("students/{studentID}/select")]
    public RedirectToActionResult SelectStudent(int studentID)
    {
        Student potentialStudent = _context.Students.FirstOrDefault(s => s.StudentId == studentID);
        if (potentialStudent != null)
        {
            HttpContext.Session.SetInt32("SelectedStudent", studentID);
        }
        return RedirectToAction("Index");
    }

    //removes student selection
    public RedirectToActionResult ClearStudentSelection()
    {
        HttpContext.Session.Remove("SelectedStudent");
        return RedirectToAction("Index");
    }

    //sets selected courseID into session after confirming existence of course
    [HttpPost("courses/{courseID}/select")]
    public RedirectToActionResult SelectCourse(int courseID)
    {
        //alternate way to check existence
        bool courseExists = _context.Courses.Any(c=>c.CourseId==courseID);
        if (courseExists)
        {
            HttpContext.Session.SetInt32("SelectedCourse", courseID);
        }
        return RedirectToAction("Index");
    }

    //removes course selection
    public RedirectToActionResult ClearCourseSelection()
    {
        HttpContext.Session.Remove("SelectedCourse");
        return RedirectToAction("Index");
    }



    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
