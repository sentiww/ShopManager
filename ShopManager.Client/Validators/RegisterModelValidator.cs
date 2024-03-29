using FluentValidation;
using ShopManager.Client.Models;

namespace ShopManager.Client.Validators;

public class RegisterModelValidator : AbstractValidator<RegisterModel>
{
    public RegisterModelValidator()
    {
        RuleFor(request => request.UserName)
            .NotEmpty()
            .Length(1, 100);

        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress();
        
        RuleFor(model => model.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Must(password => password.Any(char.IsDigit))
            .Must(password => password.Any(char.IsUpper))
            .Must(password => password.Any(char.IsLower))
            .Must(password => password.Any(char.IsNumber));
        
        RuleFor(request => request.ConfirmPassword)
            .Equal(request => request.Password)
            .WithMessage("Passwords must match.");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<RegisterModel>.CreateWithOptions((RegisterModel)model,
            strategy => strategy.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(failure => failure.ErrorMessage);
    };
}