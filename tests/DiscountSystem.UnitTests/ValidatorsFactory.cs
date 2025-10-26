using DiscountSystem.Core.Application.Commands;
using DiscountSystem.Core.Application.Validators;
using FluentValidation;

namespace DiscountSystem.UnitTests;

internal static class ValidatorsFactory
{
    public static IValidator<GenerateCodesCommand> Generate() => new GenerateCodesCommandValidator();
    public static IValidator<UseCodeCommand> Use() => new UseCodeCommandValidator();
}