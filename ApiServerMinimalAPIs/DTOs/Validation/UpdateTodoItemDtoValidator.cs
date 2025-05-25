using FluentValidation;

namespace ApiServerMinimalAPIs.DTOs.Validation;

public class UpdateTodoItemDtoValidator : AbstractValidator<UpdateTodoItemDto>
{
    public UpdateTodoItemDtoValidator()
    {
        RuleFor(e => e.Title)
            .NotEmpty()
            .MaximumLength(100);
    }
}
