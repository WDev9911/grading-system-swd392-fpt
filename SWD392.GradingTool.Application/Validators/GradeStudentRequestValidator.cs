using FluentValidation;
using SWD392.GradingTool.Application.DTOs;

namespace SWD392.GradingTool.Application.Validators;

public class GradeItemRequestValidator : AbstractValidator<GradeItemRequest>
{
    public GradeItemRequestValidator()
    {
        RuleFor(x => x.RubricId)
            .GreaterThan(0).WithMessage("RubricId phải lớn hơn 0.");

        RuleFor(x => x.Score)
            .GreaterThanOrEqualTo(0).WithMessage("Score không được nhỏ hơn 0.");
    }
}

public class GradeStudentRequestValidator : AbstractValidator<GradeStudentRequest>
{
    public GradeStudentRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0).WithMessage("StudentId phải lớn hơn 0.");

        RuleFor(x => x.Grades)
            .NotEmpty().WithMessage("Danh sách Grades không được rỗng.");

        RuleForEach(x => x.Grades).SetValidator(new GradeItemRequestValidator());
    }
}
