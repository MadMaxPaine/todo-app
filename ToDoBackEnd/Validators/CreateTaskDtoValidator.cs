using FluentValidation;
using ToDoBackEnd.Dtos;

namespace ToDoBackEnd.Validators;

public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0);
    }
}