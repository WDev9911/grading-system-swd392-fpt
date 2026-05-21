using FluentValidation;
using SWD392.GradingTool.Application.DTOs;

namespace SWD392.GradingTool.Application.Validators;

public class CreateRubricRequestValidator : AbstractValidator<CreateRubricRequest>
{
    public CreateRubricRequestValidator()
    {
        RuleFor(x => x.RubricName)
            .NotEmpty().WithMessage("RubricName không được rỗng.")
            .MaximumLength(200).WithMessage("RubricName không được vượt quá 200 ký tự.");

        RuleFor(x => x.MaxScore)
            .GreaterThan(0).WithMessage("MaxScore phải lớn hơn 0.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description không được vượt quá 1000 ký tự.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}