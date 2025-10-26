using DiscountSystem.Core.Application.Commands;
using DiscountSystem.Shared;
using FluentValidation;

namespace DiscountSystem.Core.Application.Validators;

public class GenerateCodesCommandValidator : AbstractValidator<GenerateCodesCommand>
{
    public GenerateCodesCommandValidator()
    {
        RuleFor(x => x.Count)
            .GreaterThan((ushort)0)
            .LessThanOrEqualTo((ushort)2000)
            .WithMessage(ErrorMessages.CountOutOfRange.Message);

        RuleFor(x => x.Length)
            .InclusiveBetween((byte)7, (byte)8)
            .WithMessage(ErrorMessages.LengthOutOfRange.Message);
    }
}