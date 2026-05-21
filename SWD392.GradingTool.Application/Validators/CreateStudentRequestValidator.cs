using FluentValidation;
using SWD392.GradingTool.Application.DTOs;

namespace SWD392.GradingTool.Application.Validators;

public class CreateStudentRequestValidator : AbstractValidator<CreateStudentRequest>
{
    public CreateStudentRequestValidator()
    {
        RuleFor(x => x.StudentCode)
            .NotEmpty().WithMessage("StudentCode không được rỗng.")
            .MaximumLength(50).WithMessage("StudentCode không được vượt quá 50 ký tự.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("FullName không được rỗng.")
            .MaximumLength(200).WithMessage("FullName không được vượt quá 200 ký tự.");

        RuleFor(x => x.ClassGroupId)
            .GreaterThan(0).WithMessage("ClassGroupId phải lớn hơn 0.");
    }
}