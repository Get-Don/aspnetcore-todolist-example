using FluentValidation;

namespace ApiServerMinimalAPIs.DTOs.Validation;

public class CreateTodoItemDtoValidator : AbstractValidator<CreateTodoItemDto>
{
    public CreateTodoItemDtoValidator()
    {
        RuleFor(e => e.Title)
            .NotEmpty()
            .MaximumLength(100);
    }
}
