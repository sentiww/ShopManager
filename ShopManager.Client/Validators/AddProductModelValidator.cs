using FluentValidation;
using ShopManager.Client.Models;
using ShopManager.Client.Requests;

namespace ShopManager.Client.Validators;

public class AddProductModelValidator : AbstractValidator<AddProductModel>
{
    public AddProductModelValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .Length(1, 100);

        RuleFor(request => request.Description)
            .NotEmpty()
            .Length(1, 500);

        RuleFor(request => request.Price)
            .GreaterThan(0);
        
        RuleFor(request => request.Quantity)
            .GreaterThan(-1);
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<AddProductModel>.CreateWithOptions((AddProductModel)model,
            strategy => strategy.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(failure => failure.ErrorMessage);
    };
}