﻿using System.Diagnostics;
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

    public IActionResult Index()
    {
        ViewAdminPanel viewModel = new()
        {
            AllCourses = _context.Courses.Include(c => c.StudentEnrollments).ToList(),
            AllStudents = _context.Students.Include(s => s.CourseEnrollments).ThenInclude(e => e.CourseEnrolled).ToList(),
            AllSubjects = _context.Subjects.ToList()
        };
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
        if (!ModelState.IsValid)
        {
            var message = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            Console.WriteLine(message);
        }
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

    public RedirectToActionResult ClearStudentSelection()
    {
        HttpContext.Session.Remove("SelectedStudent");
        return RedirectToAction("Index");
    }

    
    [HttpPost("courses/{courseID}/select")]
    public RedirectToActionResult SelectCourse(int courseID)
    {
        Course? potentialCourse = _context.Courses.FirstOrDefault(c => c.CourseId == courseID);
        if (potentialCourse != null)
        {
            HttpContext.Session.SetInt32("SelectedCourse", courseID);
        }
        return RedirectToAction("Index");
    }

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