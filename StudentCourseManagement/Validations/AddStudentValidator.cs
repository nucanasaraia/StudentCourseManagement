using FluentValidation;
using StudentCourseManagement.Requests;

namespace StudentCourseManagement.Validations
{
    public class AddStudentValidator : AbstractValidator<AddStudent>
    {
        public AddStudentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(2, 50)
                .WithMessage("Name lenght should be between 2 and 50");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress();
        }
    }
}
