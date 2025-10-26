using DiscountSystem.Core.Application.Commands;
using DiscountSystem.Shared;
using FluentValidation;

namespace DiscountSystem.Core.Application.Validators;

public class UseCodeCommandValidator : AbstractValidator<UseCodeCommand>
{
    public UseCodeCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage(ErrorMessages.CodeRequired.Message)
            .Length(7, 8).WithMessage(ErrorMessages.CodeLengthInvalid.Message)
            .Matches("^[A-Za-z0-9]+$").WithMessage(ErrorMessages.CodeInvalidCharacters.Message);
    }
}
