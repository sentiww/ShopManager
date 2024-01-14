using FluentValidation;
using ShopManager.Client.Models;

namespace ShopManager.Client.Validators;

public class AddDiscountModelValidator : AbstractValidator<AddDiscountModel>
{
    public AddDiscountModelValidator()
    {
        RuleFor(discount => discount.ProductId)
            .NotEmpty()
            .NotEqual(Guid.Empty);
        
        RuleFor(discount => discount.Percentage)
            .InclusiveBetween(1, 100);
        
        RuleFor(discount => discount.StartEndDate.Start)
            .NotEmpty()
            .LessThan(discount => discount.StartEndDate.End);
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<AddDiscountModel>.CreateWithOptions((AddDiscountModel)model,
            strategy => strategy.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(failure => failure.ErrorMessage);
    };
}