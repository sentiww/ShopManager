using MudBlazor;
using ShopManager.Common.Requests;

namespace ShopManager.Client.Models;

public sealed class AddDiscountModel
{
    public decimal Percentage { get; set; }
    public DateRange StartEndDate { get; set; }
    public Guid ProductId { get; set; }
}

public static class AddDiscountModelExtensions
{
    public static CreateDiscountRequest ToRequest(this AddDiscountModel model)
    {
        return new CreateDiscountRequest
        {
            Percentage = model.Percentage,
            StartDate = model.StartEndDate.Start.Value,
            EndDate = model.StartEndDate.End.Value,
            ProductId = model.ProductId
        };
    }
}