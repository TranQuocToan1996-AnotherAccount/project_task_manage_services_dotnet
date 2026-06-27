using FluentValidation;
using TaskManagement.DTO.Task;

namespace TaskManagement.Validators;

public class CreateTaskValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required");
    }
}
