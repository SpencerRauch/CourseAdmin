using System.ComponentModel.DataAnnotations;

namespace CourseAdmin.Models;

//flat view model
/*
    This model will validate that we got a subject id, while also holding onto the list of all subjects 
    for our drop down. In the controller, after validating, we will use the SubjectId to make a new Course object
    and store it in our Courses DbSet.
*/
public class ViewCourseForm
{
    [SubjectExists]
    public int SubjectId { get;set; }
    //make sure this is defaulted, we don't want to validate it
    public List<Subject>? AllSubjects { get;set; } = new();

}


//backend validation of SubjectId (making sure it is tied to a real subject)
public class SubjectExistsAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
    	// Though we have Required as a validation, sometimes we make it here anyways
    	// In which case we must first verify the value is not null before we proceed
        if(value == null)
        {
    	    // If it was, return the required error
            return new ValidationResult("Subject is required!");
        }
    
    	// This will connect us to our database since we are not in our Controller
        SchoolContext _context = (SchoolContext)validationContext.GetService(typeof(SchoolContext));
        // Check to see if there are any records of this SubjectId in our db
    	if(!_context.Subjects.Any(e => e.SubjectId == (int)value))
        {
    	    // If not, throw an error
            return new ValidationResult("Subject must exist in database");
        } else {
    	    // If yes, proceed
            return ValidationResult.Success;
        }
    }
}