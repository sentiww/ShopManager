using FluentValidation;
using ShopManager.Client.Models;

namespace ShopManager.Client.Validators;

public class AddCollectionModelValidator : AbstractValidator<AddCollectionModel>
{
    public AddCollectionModelValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .Length(1, 100);
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<AddCollectionModel>.CreateWithOptions((AddCollectionModel)model,
            strategy => strategy.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(failure => failure.ErrorMessage);
    };
}