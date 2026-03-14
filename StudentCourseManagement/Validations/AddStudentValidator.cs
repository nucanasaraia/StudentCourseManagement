using FluentValidation;
using StudentCourseManagement.Requests;

namespace StudentCourseManagement.Validations
{
    public class AddStudentValidator : AbstractValidator<AddStudent>
    {
        public AddStudentValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .Length(2, 50)
                .WithMessage("Name lenght should be between 2 and 50");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress();
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6).WithMessage("Password should be at least 6 characters long")
                .Matches("[A-Z]").WithMessage("Password should contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password should contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password should contain at least one digit")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password should contain at least one special character"); 
        }
    }
}
