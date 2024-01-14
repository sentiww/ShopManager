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
        
        RuleFor(request => request.Password)
            .NotEmpty()
            .Length(8, 100);
        
        RuleFor(request => request.ConfirmPassword)
            .Equal(request => request.Password);
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